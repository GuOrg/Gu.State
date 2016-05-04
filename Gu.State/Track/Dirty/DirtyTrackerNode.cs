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

    internal sealed class DirtyTrackerNode : IRefCountable, INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs DiffPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Diff));
        private static readonly PropertyChangedEventArgs IsDirtyPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IsDirty));
        private readonly IRefCounted<ChangeTrackerNode> xNode;
        private readonly IRefCounted<ChangeTrackerNode> yNode;
        private readonly DisposingMap<IDisposable> children = new DisposingMap<IDisposable>();
        private readonly UpdatingDiffBuilder diffBuilder;

        private DirtyTrackerNode(object x, object y, PropertiesSettings settings)
            : this(x, y, settings, new UpdatingDiffBuilder(x, y, settings))
        {
        }

        private DirtyTrackerNode(object x, object y, PropertiesSettings settings, UpdatingDiffBuilder builder)
        {
            this.xNode = ChangeTrackerNode.GetOrCreate(this, x, settings);
            this.yNode = ChangeTrackerNode.GetOrCreate(this, y, settings);
            this.xNode.Tracker.PropertyChange += this.OnTrackedPropertyChange;
            this.yNode.Tracker.PropertyChange += this.OnTrackedPropertyChange;
            this.diffBuilder = builder;

            // resetting here in case another thread updated x or y before we subscribed
            // this is perhaps slightly wasteful
            foreach (var property in settings.GetProperties(x.GetType()))
            {
                this.UpdatePropertyNode(property);
            }

            if (Is.NotifyCollections(x, y))
            {
                this.xNode.Tracker.Add += this.OnTrackedAdd;
                this.xNode.Tracker.Remove += this.OnTrackedRemove;
                this.xNode.Tracker.Replace += this.OnTrackedReplace;
                this.xNode.Tracker.Move += this.OnTrackedMove;
                this.xNode.Tracker.Reset += this.OnTrackedReset;

                this.yNode.Tracker.Add += this.OnTrackedAdd;
                this.yNode.Tracker.Remove += this.OnTrackedRemove;
                this.yNode.Tracker.Replace += this.OnTrackedReplace;
                this.yNode.Tracker.Move += this.OnTrackedMove;
                this.yNode.Tracker.Reset += this.OnTrackedReset;

                // resetting here in case another thread updated x or y before we subscribed
                // this is perhaps slightly wasteful
                this.OnTrackedReset(x, new ResetEventArgs(null, null));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler Changed;

        public bool IsDirty => this.Diff?.IsEmpty == false;

        public ValueDiff Diff => this.diffBuilder.ValueDiff;

        private object X => this.xNode.Tracker.Source;

        private IList XList => (IList)this.xNode.Tracker.Source;

        private object Y => this.yNode.Tracker.Source;

        private IList YList => (IList)this.yNode.Tracker.Source;

        private IReadOnlyCollection<PropertyInfo> TrackProperties => this.xNode.Tracker.TrackProperties;

        private PropertiesSettings Settings => this.xNode.Tracker.Settings;

        public void Dispose()
        {
            this.xNode.RemoveOwner(this);
            this.xNode.Tracker.PropertyChange -= this.OnTrackedPropertyChange;
            this.xNode.Tracker.Add -= this.OnTrackedAdd;
            this.xNode.Tracker.Remove -= this.OnTrackedRemove;
            this.xNode.Tracker.Remove -= this.OnTrackedRemove;
            this.xNode.Tracker.Move -= this.OnTrackedMove;
            this.xNode.Tracker.Reset -= this.OnTrackedReset;

            this.yNode.RemoveOwner(this);
            this.yNode.Tracker.PropertyChange -= this.OnTrackedPropertyChange;
            this.yNode.Tracker.Add -= this.OnTrackedAdd;
            this.yNode.Tracker.Remove -= this.OnTrackedRemove;
            this.yNode.Tracker.Remove -= this.OnTrackedRemove;
            this.yNode.Tracker.Move -= this.OnTrackedMove;
            this.yNode.Tracker.Reset -= this.OnTrackedReset;

            this.children.Dispose();
            this.diffBuilder.Dispose();
        }

        internal static IRefCounted<DirtyTrackerNode> GetOrCreate(object owner, object x, object y, PropertiesSettings settings)
        {
            Debug.Assert(x != null, "Cannot track null");
            Debug.Assert(x is INotifyPropertyChanged || x is INotifyCollectionChanged, "Must notify");
            Debug.Assert(y != null, "Cannot track null");
            Debug.Assert(y is INotifyPropertyChanged || y is INotifyCollectionChanged, "Must notify");
            return settings.DirtyNodes.GetOrAdd(owner, new ReferencePair(x, y), () => new DirtyTrackerNode(x, y, settings));
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

        private void UpdatePropertyNode(PropertyInfo propertyInfo)
        {
            if (this.Settings.IsIgnoringProperty(propertyInfo))
            {
                return;
            }

            var before = this.IsDirty;
            this.diffBuilder.Update(propertyInfo);

            if (this.TrackProperties.Contains(propertyInfo))
            {
                var getterAndSetter = this.Settings.GetOrCreateGetterAndSetter(propertyInfo);
                var xv = getterAndSetter.GetValue(this.X);
                var yv = getterAndSetter.GetValue(this.Y);
                var refCounted = this.CreateChildTracker(xv, yv);
                this.children.SetValue(propertyInfo, refCounted);
            }

            this.NotifyChanges(before);
        }

        private void OnTrackedAdd(object sender, AddEventArgs e)
        {
            this.UpdateIndexNode(e.Index);
        }

        private void OnTrackedRemove(object sender, RemoveEventArgs e)
        {
            var before = this.IsDirty;
            this.children.Remove(e.Index);
            this.diffBuilder.Update(e);
            this.NotifyChanges(before);
        }

        private void OnTrackedReplace(object sender, ReplaceEventArgs e)
        {
            this.UpdateIndexNode(e.Index);
        }

        private void OnTrackedMove(object sender, MoveEventArgs e)
        {
            this.children.Move(e.FromIndex, e.ToIndex);
            this.NotifyChanges(this.IsDirty);
        }

        private void OnTrackedReset(object sender, ResetEventArgs e)
        {
            var before = this.IsDirty;
            try
            {
                var maxDiffIndex = this.Diff.Diffs.OfType<IndexDiff>().Max(x => (int)x.Index);
                var max = Math.Max(maxDiffIndex, Math.Max(((IList)this.X).Count, ((IList)this.Y).Count));
                for (var i = 0; i < max; i++)
                {
                    this.UpdateIndexNode(i);
                }
            }
            finally
            {
                this.NotifyChanges(before);
            }
        }

        private void UpdateIndexNode(int index)
        {
            var before = this.IsDirty;

            var xValue = this.XList.ElementAtOrMissing(index);
            var yValue = this.YList.ElementAtOrMissing(index);
            this.diffBuilder.Update(index);
            if (IsTrackablePair(xValue, yValue, this.Settings) &&
               (this.Settings.ReferenceHandling == ReferenceHandling.Structural || this.Settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops))
            {
                var refCounted = this.CreateChildTracker(xValue, yValue);
                this.children.SetValue(index, refCounted);
            }
            else
            {
                this.children.SetValue(index, null);
            }

            this.NotifyChanges(before);
        }

        private IDisposable CreateChildTracker(object xValue, object yValue)
        {
            if (xValue == null || yValue == null)
            {
                return null;
            }

            var childNode = GetOrCreate(this, xValue, yValue, this.Settings);
            childNode.Tracker.Changed += this.OnChildChanged;
            var disposable = new Disposer(() =>
            {
                childNode.RemoveOwner(this);
                childNode.Tracker.Changed -= this.OnChildChanged;
            });

            return disposable;
        }

        private void OnChildChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void NotifyChanges(bool dirtyBefore)
        {
            this.PropertyChanged?.Invoke(this, DiffPropertyChangedEventArgs);
            this.Changed?.Invoke(this, EventArgs.Empty);

            if (this.IsDirty != dirtyBefore)
            {
                this.PropertyChanged?.Invoke(this, IsDirtyPropertyChangedEventArgs);
            }
        }
    }
}
