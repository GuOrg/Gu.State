﻿namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    internal sealed class DirtyTrackerNode : IRefCountable, INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs DiffPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Diff));
        private static readonly PropertyChangedEventArgs IsDirtyPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IsDirty));
        private readonly IRefCounted<ChangeTrackerNode> xNode;
        private readonly IRefCounted<ChangeTrackerNode> yNode;
        private readonly DisposingMap<IDisposable> children = new DisposingMap<IDisposable>();
        private readonly object gate = new object();
        private ValueDiff diff;
        private bool isBubbling;

        private DirtyTrackerNode(object x, object y, PropertiesSettings settings)
        {
            this.xNode = ChangeTrackerNode.GetOrCreate(this, x, settings);
            this.yNode = ChangeTrackerNode.GetOrCreate(this, y, settings);
            this.xNode.Tracker.PropertyChange += this.OnTrackedPropertyChange;
            this.yNode.Tracker.PropertyChange += this.OnTrackedPropertyChange;
            this.diff = DiffBy.PropertyValues(x, y, settings);
            foreach (var property in x.GetType().GetProperties(settings.BindingFlags))
            {
                this.UpdatePropertyNode(property);
            }

            if (IsNotifyingCollection(x) && IsNotifyingCollection(y))
            {
                this.OnTrackedReset(x, new ResetEventArgs(null, null));

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
                    this.OnPropertyChanged(DiffPropertyChangedEventArgs);
                    this.Changed?.Invoke(this, EventArgs.Empty);
                    if (!this.isBubbling)
                    {
                        this.BubbleChange?.Invoke(this, this);
                    }

                    if (value == null || before == null)
                    {
                        this.OnPropertyChanged(IsDirtyPropertyChangedEventArgs);
                    }
                }
            }
        }

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
        }

        internal static IRefCounted<DirtyTrackerNode> GetOrCreate(object owner, object x, object y, PropertiesSettings settings)
        {
            Debug.Assert(x != null, "Cannot track null");
            Debug.Assert(x is INotifyPropertyChanged || x is INotifyCollectionChanged, "Must notify");
            Debug.Assert(y != null, "Cannot track null");
            Debug.Assert(y is INotifyPropertyChanged || y is INotifyCollectionChanged, "Must notify");
            return settings.DirtyNodes.GetOrAdd(owner, new ReferencePair(x, y), () => new DirtyTrackerNode(x, y, settings));
        }

        private static bool IsNotifyingCollection(object o)
        {
            return o is INotifyCollectionChanged && o is IList;
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

        private static object GetValue(IList source, int index)
        {
            if (source == null || index >= source.Count)
            {
                return PaddedPairs.MissingItem;
            }

            return source[index];
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

            var xValue = propertyInfo.GetValue(this.xNode.Tracker.Source);
            var yValue = propertyInfo.GetValue(this.yNode.Tracker.Source);

            if (this.TrackProperties.Contains(propertyInfo) &&
               (this.Settings.ReferenceHandling == ReferenceHandling.Structural || this.Settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops))
            {
                var refCounted = this.CreateChild(xValue, yValue, propertyInfo);
                this.children.SetValue(propertyInfo, refCounted);
            }

            var propertyValueDiff = xValue != null && yValue != null
                                        ? DiffBy.PropertyValues(xValue, yValue, this.Settings)
                                        : xValue != null || yValue != null
                                              ? new ValueDiff(xValue, yValue)
                                              : null;

            lock (this.gate)
            {
                this.Diff = propertyValueDiff == null
                                ? this.diff.Without(propertyInfo)
                                : this.diff.With(this.xNode.Tracker.Source, this.yNode.Tracker.Source, propertyInfo, propertyValueDiff);
            }
        }

        private void OnTrackedAdd(object sender, AddEventArgs e)
        {
            this.UpdateIndexNode(e.Index);
        }

        private void OnTrackedRemove(object sender, RemoveEventArgs e)
        {
            this.children.Remove(e.Index);
            var xValue = GetValue((IList)this.xNode.Tracker.Source, e.Index);
            var yValue = GetValue((IList)this.yNode.Tracker.Source, e.Index);
            var indexValueDiff = this.CreateIndexValueDiff(xValue, yValue);

            lock (this.gate)
            {
                this.Diff = indexValueDiff == null
                                ? this.diff.Without(e.Index)
                                : this.diff.With(this.xNode.Tracker.Source, this.yNode.Tracker.Source, e.Index, indexValueDiff);
            }
        }

        private void OnTrackedReplace(object sender, ReplaceEventArgs e)
        {
            this.UpdateIndexNode(e.Index);
        }

        private void OnTrackedMove(object sender, MoveEventArgs e)
        {
            this.OnTrackedReplace(sender, new ReplaceEventArgs(e.FromIndex));
            this.OnTrackedReplace(sender, new ReplaceEventArgs(e.ToIndex));
        }

        private void OnTrackedReset(object sender, ResetEventArgs e)
        {
            for (int i = 0; i < Math.Max(((IList)this.xNode.Tracker.Source).Count, ((IList)this.yNode.Tracker.Source).Count); i++)
            {
                this.UpdateIndexNode(i);
            }
        }

        private void UpdateIndexNode(int index)
        {
            var xValue = GetValue((IList)this.xNode.Tracker.Source, index);
            var yValue = GetValue((IList)this.yNode.Tracker.Source, index);

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
                                : this.diff.With(this.xNode.Tracker.Source, this.yNode.Tracker.Source, index, indexValueDiff);
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

            return DiffBy.PropertyValues(xValue, yValue, this.Settings);
        }

        private IDisposable CreateChild(object xValue, object yValue, object key)
        {
            if (xValue == null || yValue == null)
            {
                return null;
            }

            var childNode = GetOrCreate(this, xValue, yValue, this.Settings);
            EventHandler<DirtyTrackerNode> trackerOnBubbleChange = (sender, args) => this.OnBubbleChange(sender, args, key);
            childNode.Tracker.BubbleChange += trackerOnBubbleChange;
            var disposable = new Disposer(() =>
            {
                childNode.RemoveOwner(this);
                childNode.Tracker.BubbleChange -= trackerOnBubbleChange;
            });
            return disposable;
        }

        private void OnBubbleChange(object sender, DirtyTrackerNode originalSource, object key)
        {
            if (ReferenceEquals(this, originalSource))
            {
                return;
            }

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
                            : this.diff.With(this.xNode.Tracker.Source, this.yNode.Tracker.Source, propertyInfo, node.diff);
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
                        : this.diff.With(this.xNode.Tracker.Source, this.yNode.Tracker.Source, index, node.diff);
                }
                finally
                {
                    this.isBubbling = false;
                }
            }

            this.BubbleChange?.Invoke(this, originalSource);
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }
    }
}
