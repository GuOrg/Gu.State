namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal sealed class DirtyTrackerNode : IDirtyTracker, IInitialize<DirtyTrackerNode>
    {
        private static readonly PropertyChangedEventArgs DiffPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Diff));
        private static readonly PropertyChangedEventArgs IsDirtyPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IsDirty));

        private readonly IRefCounted<ReferencePair> refCountedPair;
        private readonly IRefCounted<ChangeNode> xNode;
        private readonly IRefCounted<ChangeNode> yNode;
        private readonly IBorrowed<DisposingMap<IUnsubscriber<IRefCounted<DirtyTrackerNode>>>> children;
        private readonly IRefCounted<DiffBuilder> refcountedDiffBuilder;

        private bool isDirty;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirtyTrackerNode"/> class.
        /// A call to Initialize is needed after the ctor due to that we need to fetch child nodes and the graph can contain self
        /// </summary>
        private DirtyTrackerNode(IRefCounted<ReferencePair> refCountedPair, PropertiesSettings settings, bool isRoot)
        {
            this.refCountedPair = refCountedPair;
            var x = refCountedPair.Value.X;
            var y = refCountedPair.Value.Y;
            this.children = DisposingMap<IUnsubscriber<IRefCounted<DirtyTrackerNode>>>.Borrow();
            this.xNode = ChangeNode.GetOrCreate(x, settings, isRoot);
            this.yNode = ChangeNode.GetOrCreate(y, settings, isRoot);
            this.xNode.Value.PropertyChange += this.OnTrackedPropertyChange;
            this.yNode.Value.PropertyChange += this.OnTrackedPropertyChange;

            this.IsTrackingCollectionItems = Is.Enumerable(x, y) &&
                                 !settings.IsImmutable(x.GetType().GetItemType()) &&
                                 !settings.IsImmutable(y.GetType().GetItemType());

            if (Is.NotifyCollections(x, y))
            {
                this.xNode.Value.Add += this.OnTrackedAdd;
                this.xNode.Value.Remove += this.OnTrackedRemove;
                this.xNode.Value.Replace += this.OnTrackedReplace;
                this.xNode.Value.Move += this.OnTrackedMove;
                this.xNode.Value.Reset += this.OnTrackedReset;

                this.yNode.Value.Add += this.OnTrackedAdd;
                this.yNode.Value.Remove += this.OnTrackedRemove;
                this.yNode.Value.Replace += this.OnTrackedReplace;
                this.yNode.Value.Move += this.OnTrackedMove;
                this.yNode.Value.Reset += this.OnTrackedReset;
            }

            var builder = DiffBuilder.GetOrCreate(x, y, settings);
            builder.Value.UpdateDiffs(x, y, settings);
            builder.Value.Refresh();
            this.refcountedDiffBuilder = builder;
            this.isDirty = !this.Builder.IsEmpty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal event EventHandler<TrackerChangedEventArgs<DirtyTrackerNode>> Changed;

        public PropertiesSettings Settings => this.xNode.Value.Settings;

        public bool IsDirty
        {
            get
            {
                return this.isDirty;
            }

            private set
            {
                if (value == this.isDirty)
                {
                    return;
                }

                this.isDirty = value;
                this.PropertyChanged?.Invoke(this, IsDirtyPropertyChangedEventArgs);
            }
        }

        public ValueDiff Diff => this.Builder?.CreateValueDiffOrNull();

        private bool IsTrackingCollectionItems { get; }

        private DiffBuilder Builder => this.refcountedDiffBuilder?.Value;

        internal object X => this.xNode.Value.Source;

        internal IList XList => (IList)this.X;

        internal object Y => this.yNode.Value.Source;

        internal IList YList => (IList)this.Y;

        private IReadOnlyCollection<PropertyInfo> TrackProperties => this.xNode.Value.TrackProperties;

        private DisposingMap<IUnsubscriber<IRefCounted<DirtyTrackerNode>>> Children => this.children.Value;

        public void Dispose()
        {
            this.xNode.Value.PropertyChange -= this.OnTrackedPropertyChange;
            this.xNode.Value.Add -= this.OnTrackedAdd;
            this.xNode.Value.Remove -= this.OnTrackedRemove;
            this.xNode.Value.Remove -= this.OnTrackedRemove;
            this.xNode.Value.Move -= this.OnTrackedMove;
            this.xNode.Value.Reset -= this.OnTrackedReset;
            this.xNode.Dispose();

            this.yNode.Value.PropertyChange -= this.OnTrackedPropertyChange;
            this.yNode.Value.Add -= this.OnTrackedAdd;
            this.yNode.Value.Remove -= this.OnTrackedRemove;
            this.yNode.Value.Remove -= this.OnTrackedRemove;
            this.yNode.Value.Move -= this.OnTrackedMove;
            this.yNode.Value.Reset -= this.OnTrackedReset;
            this.yNode.Dispose();

            this.children.Dispose();
            this.refCountedPair.Dispose();
            this.refcountedDiffBuilder.Dispose();
        }

        // Initialize is needed here as we can't get recursive trackers in the ctor
        // Call the ctor like new DirtyTrackerNode(pair, settings).Initialize()
        DirtyTrackerNode IInitialize<DirtyTrackerNode>.Initialize()
        {
            foreach (var propertyInfo in this.TrackProperties)
            {
                this.UpdatePropertyChildNode(propertyInfo);
            }

            if (this.IsTrackingCollectionItems)
            {
                for (var i = 0; i < Math.Max(this.XList.Count, this.YList.Count); i++)
                {
                    this.UpdateIndexChildNode(i);
                }
            }

            return this;
        }

        internal static IRefCounted<DirtyTrackerNode> GetOrCreate(object x, object y, PropertiesSettings settings, bool isRoot)
        {
            Debug.Assert(x != null, "Cannot track null");
            Debug.Assert(x is INotifyPropertyChanged || x is INotifyCollectionChanged, "Must notify");
            Debug.Assert(y != null, "Cannot track null");
            Debug.Assert(y is INotifyPropertyChanged || y is INotifyCollectionChanged, "Must notify");
            return TrackerCache.GetOrAdd(
                x,
                y,
                settings,
                pair => new DirtyTrackerNode(pair, settings, isRoot));
        }

        private static bool IsTrackablePair(object x, object y, PropertiesSettings settings)
        {
            if (IsNullOrMissing(x) || IsNullOrMissing(y))
            {
                return false;
            }

            return !settings.IsImmutable(x.GetType()) && !settings.IsImmutable(y.GetType());
        }

        private static bool IsNullOrMissing(object x)
        {
            return x == null || x == PaddedPairs.MissingItem;
        }

        private void OnTrackedPropertyChange(object sender, PropertyChangeEventArgs e)
        {
            this.UpdatePropertyChildNode(e.PropertyInfo);
            //// we create the builder after subscribing so no guarantee that we have a builder if an event fires before the ctor is finished.
            if (this.Builder == null ||
                this.Settings.IsIgnoringProperty(e.PropertyInfo))
            {
                return;
            }

            this.Builder.UpdateMemberDiff(this.X, this.Y, e.PropertyInfo, this.Settings);
            this.TryRefreshAndNotify(e);
        }

        private void UpdatePropertyChildNode(PropertyInfo propertyInfo)
        {
            if (this.Settings.IsIgnoringProperty(propertyInfo))
            {
                return;
            }

            if (this.TrackProperties.Contains(propertyInfo) &&
               (this.Settings.ReferenceHandling == ReferenceHandling.Structural))
            {
                var getter = this.Settings.GetOrCreateGetterAndSetter(propertyInfo);
                var xValue = getter.GetValue(this.X);
                var yValue = getter.GetValue(this.Y);
                IRefCounted<DirtyTrackerNode> childNode;
                if (this.TrCreateChild(xValue, yValue, out childNode))
                {
                    EventHandler<TrackerChangedEventArgs<DirtyTrackerNode>> onChanged = (sender, args) => this.OnChildChanged(sender, args, propertyInfo);
                    childNode.Value.Changed += onChanged;
                    var child = childNode.UnsubscribeAndDispose(x => x.Value.Changed -= onChanged);
                    this.Children.SetValue(propertyInfo, child);
                }
                else
                {
                    this.Children.Remove(propertyInfo);
                }
            }
        }

        private void OnTrackedAdd(object sender, AddEventArgs e)
        {
            this.UpdateIndexChildNode(e.Index);
            this.UpdateIndexDiff(e.Index);
            this.TryRefreshAndNotify(e);
        }

        private void OnTrackedRemove(object sender, RemoveEventArgs e)
        {
            this.UpdateIndexChildNode(e.Index);
            this.UpdateIndexDiff(e.Index);
            this.TryRefreshAndNotify(e);
        }

        private void OnTrackedReplace(object sender, ReplaceEventArgs e)
        {
            this.UpdateIndexChildNode(e.Index);
            this.UpdateIndexDiff(e.Index);
            this.TryRefreshAndNotify(e);
        }

        private void OnTrackedMove(object sender, MoveEventArgs e)
        {
            this.UpdateIndexChildNode(e.FromIndex);
            this.UpdateIndexDiff(e.FromIndex);
            this.UpdateIndexChildNode(e.ToIndex);
            this.UpdateIndexDiff(e.ToIndex);
            this.TryRefreshAndNotify(e);
        }

        private void OnTrackedReset(object sender, ResetEventArgs e)
        {
            this.Builder?.ClearIndexDiffs();
            this.Children.ClearIndexTrackers();
            var max = Math.Max(this.XList.Count, this.YList.Count);
            for (var i = 0; i < max; i++)
            {
                this.UpdateIndexChildNode(i);
                this.UpdateIndexDiff(i);
            }

            this.TryRefreshAndNotify(e);
        }

        private void UpdateIndexChildNode(int index)
        {
            if (!this.IsTrackingCollectionItems)
            {
                return;
            }

            var xValue = this.XList.ElementAtOrMissing(index);
            var yValue = this.YList.ElementAtOrMissing(index);

            IRefCounted<DirtyTrackerNode> childNode;
            if (this.TrCreateChild(xValue, yValue, out childNode))
            {
                EventHandler<TrackerChangedEventArgs<DirtyTrackerNode>> onChanged = (sender, args) => this.OnChildChanged(sender, args, index);
                childNode.Value.Changed += onChanged;
                var child = childNode.UnsubscribeAndDispose(x => x.Value.Changed -= onChanged);
                this.Children.SetValue(index, child);
            }
            else
            {
                this.Children.SetValue(index, null);
            }
        }

        private void UpdateIndexDiff(int index)
        {
            // we create the builder after subscribing so no guarantee that we have a builder if an event fires before the ctor is finished.
            if (this.Builder == null)
            {
                return;
            }

            var xValue = this.XList.ElementAtOrMissing(index);
            var yValue = this.YList.ElementAtOrMissing(index);
            this.Builder.UpdateCollectionItemDiff(xValue, yValue, index, this.Settings);
        }

        private bool TrCreateChild(object xValue, object yValue, out IRefCounted<DirtyTrackerNode> childNode)
        {
            if (!IsTrackablePair(xValue, yValue, this.Settings) ||
                this.Settings.ReferenceHandling != ReferenceHandling.Structural)
            {
                childNode = null;
                return false;
            }

            childNode = GetOrCreate(xValue, yValue, this.Settings, false);
            return true;
        }

        // ReSharper disable once UnusedParameter.Local
        private void OnChildChanged(object _, TrackerChangedEventArgs<DirtyTrackerNode> e, int index)
        {
            if (e.Contains(this) || this.Builder == null)
            {
                return;
            }

            this.Builder.UpdateCollectionItemDiff(this.XList.ElementAtOrMissing(index), this.YList.ElementAtOrMissing(index), index, this.Settings);
            this.Builder.Refresh();
            this.PropertyChanged?.Invoke(this, DiffPropertyChangedEventArgs);
            this.IsDirty = !this.Builder.IsEmpty;
            this.Changed?.Invoke(this, e.With(this, index));
        }

        private void OnChildChanged(object _, TrackerChangedEventArgs<DirtyTrackerNode> e, PropertyInfo property)
        {
            if (e.Contains(this) || this.Builder == null)
            {
                return;
            }

            if (this.Settings.IsIgnoringProperty(property))
            {
                return;
            }

            this.Builder.UpdateMemberDiff(this.X, this.Y, property, this.Settings);
            this.Builder.Refresh();
            this.PropertyChanged?.Invoke(this, DiffPropertyChangedEventArgs);
            this.IsDirty = !this.Builder.IsEmpty;
            this.Changed?.Invoke(this, e.With(this, property));
        }

        private void TryRefreshAndNotify<T>(T e)
            where T : IRootChangeEventArgs
        {
            if (this.Builder?.TryRefresh() == true)
            {
                this.PropertyChanged?.Invoke(this, DiffPropertyChangedEventArgs);
                this.IsDirty = !this.Builder.IsEmpty;
                this.Changed?.Invoke(this, RootChangeEventArgs.Create(this, e));
            }
        }
    }
}
