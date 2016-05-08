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

    internal sealed class DirtyTrackerNode : IDisposable, INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs DiffPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Diff));
        private static readonly PropertyChangedEventArgs IsDirtyPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IsDirty));

        private readonly IRefCounted<ReferencePair> refCountedPair;
        private readonly IRefCounted<ChangeTrackerNode> xNode;
        private readonly IRefCounted<ChangeTrackerNode> yNode;
        private readonly DisposingMap<IDisposable> children = new DisposingMap<IDisposable>();
        private readonly IRefCounted<DiffBuilder> refcountedDiffBuilder;

        private bool isChanging;
        private bool isResetting;

        private DirtyTrackerNode(IRefCounted<ReferencePair> refCountedPair, PropertiesSettings settings)
        {
            this.refCountedPair = refCountedPair;
            var x = refCountedPair.Value.X;
            var y = refCountedPair.Value.Y;
            this.xNode = ChangeTrackerNode.GetOrCreate(x, settings);
            this.yNode = ChangeTrackerNode.GetOrCreate(y, settings);
            this.xNode.Value.PropertyChange += this.OnTrackedPropertyChange;
            this.yNode.Value.PropertyChange += this.OnTrackedPropertyChange;

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

            var builder = DiffBuilder.Create(x, y, settings);
            builder.Value.UpdateDiffs(x, y, settings);
            this.refcountedDiffBuilder = builder;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal event EventHandler<DirtyTrackerChangedEventArgs> Changed;

        public bool IsDirty => this.Diff != null;

        public ValueDiff Diff => this.Builder?.CreateValueDiff();

        private DiffBuilder Builder => this.refcountedDiffBuilder?.Value;

        private object X => this.xNode.Value.Source;

        private IList XList => (IList)this.X;

        private object Y => this.yNode.Value.Source;

        private IList YList => (IList)this.Y;

        private IReadOnlyCollection<PropertyInfo> TrackProperties => this.xNode.Value.TrackProperties;

        private PropertiesSettings Settings => this.xNode.Value.Settings;

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

        internal static IRefCounted<DirtyTrackerNode> GetOrCreate(object x, object y, PropertiesSettings settings)
        {
            Debug.Assert(x != null, "Cannot track null");
            Debug.Assert(x is INotifyPropertyChanged || x is INotifyCollectionChanged, "Must notify");
            Debug.Assert(y != null, "Cannot track null");
            Debug.Assert(y is INotifyPropertyChanged || y is INotifyCollectionChanged, "Must notify");
            return TrackerCache.GetOrAdd(x, y, settings, pair => new DirtyTrackerNode(pair, settings));
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
            this.UpdatePropertyNode(e.PropertyInfo);
        }

        private bool UpdatePropertyNode(PropertyInfo propertyInfo)
        {
            if (this.Settings.IsIgnoringProperty(propertyInfo))
            {
                return false;
            }

            if (this.TrackProperties.Contains(propertyInfo) &&
               (this.Settings.ReferenceHandling == ReferenceHandling.Structural || this.Settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops))
            {
                var getter = this.Settings.GetOrCreateGetterAndSetter(propertyInfo);
                var xValue = getter.GetValue(this.X);
                var yValue = getter.GetValue(this.Y);
                var refCounted = this.CreateChild(xValue, yValue, propertyInfo);
                this.children.SetValue(propertyInfo, refCounted);
            }

            return this.TryNotifyPropertyChange(propertyInfo, true);
        }

        private bool TryNotifyPropertyChange(PropertyInfo propertyInfo, bool needsRefresh)
        {
            // we create the builder after subscribing so no guarantee that we have a builder if an event fires before the ctor is finished.
            if (this.Builder == null)
            {
                return false;
            }

            var dirtyBefore = this.IsDirty;
            this.Builder.UpdateMemberDiff(this.X, this.Y, propertyInfo, this.Settings);
            return this.TryNotifyChanges(dirtyBefore, propertyInfo, needsRefresh);
        }

        private void OnTrackedAdd(object sender, AddEventArgs e)
        {
            this.TryUpdateIndexNode(e.Index);
        }

        private void OnTrackedRemove(object sender, RemoveEventArgs e)
        {
            this.children.Remove(e.Index);

            // we create the builder after subscribing so no guarantee that we have a builder if an event fires before the ctor is finished.
            if (this.refcountedDiffBuilder == null)
            {
                return;
            }

            throw new NotImplementedException("message");
        }

        private void OnTrackedReplace(object sender, ReplaceEventArgs e)
        {
            this.TryUpdateIndexNode(e.Index);
        }

        private void OnTrackedMove(object sender, MoveEventArgs e)
        {
            var dirtyBefore = this.IsDirty;
            this.isResetting = true;
            try
            {
                this.OnTrackedReplace(sender, new ReplaceEventArgs(e.FromIndex));
                this.OnTrackedReplace(sender, new ReplaceEventArgs(e.ToIndex));
            }
            finally
            {
                this.isResetting = false;
                this.TryNotifyChanges(dirtyBefore, e, true);
            }
        }

        private void OnTrackedReset(object sender, ResetEventArgs e)
        {
            throw new NotImplementedException("message");

            //var before = this.IsDirty;
            //this.isResetting = true;
            //try
            //{
            //    var maxDiffIndex = this.refcountedDiffBuilder.Value.Diffs.OfType<IndexDiff>().Max(x => (int)x.Index) + 1 ?? 0;
            //    var max = Math.Max(maxDiffIndex, Math.Max(this.XList.Count, this.YList.Count));
            //    for (var i = 0; i < max; i++)
            //    {
            //        this.TryUpdateIndexNode(i);
            //    }
            //}
            //finally
            //{
            //    this.isResetting = false;
            //    this.TryNotifyChanges(before);
            //}
        }

        private bool TryUpdateIndexNode(int index)
        {
            var xValue = this.XList.ElementAtOrMissing(index);
            var yValue = this.YList.ElementAtOrMissing(index);

            if (IsTrackablePair(xValue, yValue, this.Settings) &&
               (this.Settings.ReferenceHandling == ReferenceHandling.Structural || this.Settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops))
            {
                var refCounted = this.CreateChild(xValue, yValue, index);
                this.children.SetValue(index, refCounted);
            }
            else
            {
                this.children.SetValue(index, null);
            }

            // we create the builder after subscribing so no guarantee that we have a builder if an event fires before the ctor is finished.
            if (this.Builder == null)
            {
                return false;
            }

            var dirtyBefore = this.IsDirty;
            this.Builder.UpdateIndexDiff(this.X, this.Y, index, this.Settings);
            return this.TryNotifyChanges(dirtyBefore, index, true);
        }

        private IUnsubscriber CreateChild(object xValue, object yValue, object key)
        {
            if (xValue == null || yValue == null)
            {
                return null;
            }

            var childNode = GetOrCreate(xValue, yValue, this.Settings);
            EventHandler<DirtyTrackerChangedEventArgs> onChanged = (sender, args) => this.OnChildChanged(sender, args, key);
            childNode.Value.Changed += onChanged;
            return childNode.AsUnsubscribeOnDispose(x => x.Value.Changed -= onChanged);
        }

        // ReSharper disable once UnusedParameter.Local
        private void OnChildChanged(object _, DirtyTrackerChangedEventArgs e, object key)
        {
            if (e.Contains(this))
            {
                return;
            }

            var propertyInfo = key as PropertyInfo;
            if (propertyInfo != null)
            {
                if (this.Settings.IsIgnoringProperty(propertyInfo))
                {
                    return;
                }

                if (this.TryNotifyPropertyChange(propertyInfo, false))
                {
                    this.Changed?.Invoke(this, e.With(this, key));
                }
            }
            else
            {
                throw new NotImplementedException("message");

                //var index = (int)key;
                //this.isChanging = true;
                //try
                //{
                //    this.Diff = node.refcountedDiffBuilder == null
                //        ? this.refcountedDiffBuilder.Without(index)
                //        : this.refcountedDiffBuilder.With(this.X, this.Y, index, node.refcountedDiffBuilder);
                //}
                //finally
                //{
                //    this.isChanging = false;
                //}
            }
        }

        private bool TryNotifyChanges(bool dirtyBefore, object propertyOrIndex, bool needsRefresh)
        {
            if (needsRefresh &&
                !this.refcountedDiffBuilder.Value.TryRefresh(this.Settings))
            {
                return false;
            }

            this.PropertyChanged?.Invoke(this, DiffPropertyChangedEventArgs);
            if (this.IsDirty != dirtyBefore)
            {
                this.PropertyChanged?.Invoke(this, IsDirtyPropertyChangedEventArgs);
            }

            if (!this.isChanging)
            {
                this.Changed?.Invoke(this, new DirtyTrackerChangedEventArgs(this, propertyOrIndex));
            }

            return true;
        }
    }
}
