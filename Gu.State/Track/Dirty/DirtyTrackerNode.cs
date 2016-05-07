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
        private readonly IRefCounted<ChangeTrackerNode> xNode;
        private readonly IRefCounted<ChangeTrackerNode> yNode;
        private readonly DisposingMap<IDisposable> children = new DisposingMap<IDisposable>();
        private readonly object gate = new object();
        private ValueDiff diff;
        private bool isBubbling;
        private bool isBatchUpdating;

        private DirtyTrackerNode(object x, object y, PropertiesSettings settings)
        {
            this.xNode = ChangeTrackerNode.GetOrCreate(x, settings);
            this.yNode = ChangeTrackerNode.GetOrCreate(y, settings);
            this.xNode.Value.PropertyChange += this.OnTrackedPropertyChange;
            this.yNode.Value.PropertyChange += this.OnTrackedPropertyChange;

            this.diff = DiffBy.PropertyValuesOrNull(x, y, settings);
            foreach (var property in x.GetType().GetProperties(settings.BindingFlags))
            {
                this.UpdatePropertyNode(property);
            }

            if (Is.NotifyCollections(x, y))
            {
                this.OnTrackedReset(x, new ResetEventArgs(null, null));

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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler Changed;

        public event EventHandler<DirtyTrackerNode> BubbleChange;

        public bool IsDirty => this.Diff != null;

        public ValueDiff Diff
        {
            get
            {
                return this.diff;
            }

            private set
            {
                lock (this.gate)
                {
                    if (Equals(value, this.diff))
                    {
                        return;
                    }

                    var before = this.diff;
                    this.diff = value;
                    if (!this.isBatchUpdating)
                    {
                        this.NotifyChanges(this.diff, before);
                    }
                }
            }
        }

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
        }

        internal static IRefCounted<DirtyTrackerNode> GetOrCreate(object x, object y, PropertiesSettings settings)
        {
            Debug.Assert(x != null, "Cannot track null");
            Debug.Assert(x is INotifyPropertyChanged || x is INotifyCollectionChanged, "Must notify");
            Debug.Assert(y != null, "Cannot track null");
            Debug.Assert(y is INotifyPropertyChanged || y is INotifyCollectionChanged, "Must notify");
            return TrackerCache.GetOrAdd(x, y, settings, () => new DirtyTrackerNode(x, y, settings));
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

            var getter = this.Settings.GetOrCreateGetterAndSetter(propertyInfo);
            var xValue = getter.GetValue(this.X);
            var yValue = getter.GetValue(this.Y);

            if (this.TrackProperties.Contains(propertyInfo) &&
               (this.Settings.ReferenceHandling == ReferenceHandling.Structural || this.Settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops))
            {
                var refCounted = this.CreateChild(xValue, yValue, propertyInfo);
                this.children.SetValue(propertyInfo, refCounted);
            }

            var propertyValueDiff = xValue != null && yValue != null
                                        ? DiffBy.PropertyValuesOrNull(xValue, yValue, this.Settings)
                                        : xValue != null || yValue != null
                                              ? new ValueDiff(xValue, yValue)
                                              : null;

            lock (this.gate)
            {
                this.Diff = propertyValueDiff == null
                                ? this.diff.Without(propertyInfo)
                                : this.diff.With(this.X, this.Y, propertyInfo, propertyValueDiff);
            }
        }

        private void OnTrackedAdd(object sender, AddEventArgs e)
        {
            this.UpdateIndexNode(e.Index);
        }

        private void OnTrackedRemove(object sender, RemoveEventArgs e)
        {
            this.children.Remove(e.Index);
            var xValue = this.XList.ElementAtOrMissing(e.Index);
            var yValue = this.YList.ElementAtOrMissing(e.Index);
            var indexValueDiff = this.CreateIndexValueDiff(xValue, yValue);

            lock (this.gate)
            {
                this.Diff = indexValueDiff == null
                                ? this.diff.Without(e.Index)
                                : this.diff.With(this.X, this.Y, e.Index, indexValueDiff);
            }
        }

        private void OnTrackedReplace(object sender, ReplaceEventArgs e)
        {
            this.UpdateIndexNode(e.Index);
        }

        private void OnTrackedMove(object sender, MoveEventArgs e)
        {
            var before = this.diff;
            this.isBatchUpdating = true;
            try
            {
                this.OnTrackedReplace(sender, new ReplaceEventArgs(e.FromIndex));
                this.OnTrackedReplace(sender, new ReplaceEventArgs(e.ToIndex));
            }
            finally
            {
                this.isBatchUpdating = false;
                this.NotifyChanges(before, this.diff);
            }
        }

        private void OnTrackedReset(object sender, ResetEventArgs e)
        {
            var before = this.diff;
            this.isBatchUpdating = true;
            try
            {
                var maxDiffIndex = this.diff?.Diffs.OfType<IndexDiff>().Max(x => (int)x.Index) + 1 ?? 0;
                var max = Math.Max(maxDiffIndex, Math.Max(this.XList.Count, this.YList.Count));
                for (var i = 0; i < max; i++)
                {
                    this.UpdateIndexNode(i);
                }
            }
            finally
            {
                this.isBatchUpdating = false;
                this.NotifyChanges(before, this.diff);
            }
        }

        private void UpdateIndexNode(int index)
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

            var indexValueDiff = this.CreateIndexValueDiff(xValue, yValue);

            lock (this.gate)
            {
                this.Diff = indexValueDiff == null
                                ? this.diff.Without(index)
                                : this.diff.With(this.X, this.Y, index, indexValueDiff);
            }
        }

        private ValueDiff CreateIndexValueDiff(object xValue, object yValue)
        {
            if ((xValue == PaddedPairs.MissingItem && yValue == PaddedPairs.MissingItem) ||
                (xValue == null && yValue == null))
            {
                return null;
            }

            if (IsNullOrMissing(xValue) || IsNullOrMissing(yValue))
            {
                return new ValueDiff(xValue, yValue);
            }

            return DiffBy.PropertyValuesOrNull(xValue, yValue, this.Settings);
        }

        private IDisposable CreateChild(object xValue, object yValue, object key)
        {
            if (xValue == null || yValue == null)
            {
                return null;
            }

            var childNode = GetOrCreate(xValue, yValue, this.Settings);
            EventHandler<DirtyTrackerNode> trackerOnBubbleChange = (sender, args) => this.OnBubbleChange(sender, args, key);
            childNode.Value.BubbleChange += trackerOnBubbleChange;
            var disposable = new Disposer(() =>
            {
                childNode.Value.BubbleChange -= trackerOnBubbleChange;
                childNode.Dispose();
            });
            return disposable;
        }

        private void OnBubbleChange(object sender, DirtyTrackerNode originalSource, object key)
        {
            var node = (DirtyTrackerNode)sender;
            var propertyInfo = key as PropertyInfo;
            if (propertyInfo != null)
            {
                lock (this.gate)
                {
                    this.isBubbling = true;
                    try
                    {
                        this.Diff = node.diff == null
                            ? this.diff.Without(propertyInfo)
                            : this.diff.With(this.X, this.Y, propertyInfo, node.diff);
                    }
                    finally
                    {
                        this.isBubbling = false;
                    }
                }
            }
            else
            {
                var index = (int)key;
                this.isBubbling = true;
                try
                {
                    this.Diff = node.diff == null
                        ? this.diff.Without(index)
                        : this.diff.With(this.X, this.Y, index, node.diff);
                }
                finally
                {
                    this.isBubbling = false;
                }
            }

            if (!ReferenceEquals(this, originalSource))
            {
                this.BubbleChange?.Invoke(this, originalSource);
            }
        }

        private void NotifyChanges(ValueDiff value, ValueDiff before)
        {
            if (Equals(before, value))
            {
                return;
            }

            this.PropertyChanged?.Invoke(this, DiffPropertyChangedEventArgs);
            this.Changed?.Invoke(this, EventArgs.Empty);
            if (!this.isBubbling)
            {
                this.BubbleChange?.Invoke(this, this);
            }

            if (value == null || before == null)
            {
                this.PropertyChanged?.Invoke(this, IsDirtyPropertyChangedEventArgs);
            }
        }
    }
}
