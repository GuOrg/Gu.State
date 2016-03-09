namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    public partial class DirtyTracker<T>
    {
        private sealed class ItemsDirtyTracker : IDisposable
        {
            private readonly INotifyCollectionChanged x;
            private readonly INotifyCollectionChanged y;
            private readonly DirtyTracker<T> parent;
            private readonly DisposingList<IDirtyTrackerNode> itemTrackers;

            private ItemsDirtyTracker(INotifyCollectionChanged x, INotifyCollectionChanged y, DirtyTracker<T> parent)
            {
                this.x = x;
                this.y = y;
                this.parent = parent;
                this.x.CollectionChanged += this.OnTrackedCollectionChanged;
                this.y.CollectionChanged += this.OnTrackedCollectionChanged;
                var xList = (IList)x;
                var yList = (IList)y;
                bool anyDirty = false;
                this.itemTrackers = new DisposingList<IDirtyTrackerNode>();
                for (int i = 0; i < Math.Max(xList.Count, yList.Count); i++)
                {
                    var itemTracker = this.CreateItemTracker(i);
                    this.itemTrackers[i] = itemTracker;
                    anyDirty |= itemTracker.IsDirty;
                }

                if (anyDirty)
                {
                    parent.diff.Add(ItemDirtyTracker.IndexerProperty);
                }
            }

            public bool IsDirty => this.itemTrackers.Exists(it => it?.IsDirty == true);

            public void Dispose()
            {
                this.x.CollectionChanged -= this.OnTrackedCollectionChanged;
                this.y.CollectionChanged -= this.OnTrackedCollectionChanged;
                this.itemTrackers?.Dispose();
            }

            internal static ItemsDirtyTracker Create(object x, object y, DirtyTracker<T> parent)
            {
                var xCollectionChanged = x as INotifyCollectionChanged;
                var yCollectionChanged = y as INotifyCollectionChanged;
                if (xCollectionChanged != null && yCollectionChanged != null)
                {
                    return new ItemsDirtyTracker(xCollectionChanged, yCollectionChanged, parent);
                }

                return null;
            }

            private void OnTrackedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                var before = this.parent.diff.Count;
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        for (int i = e.NewStartingIndex; i < e.NewStartingIndex + e.NewItems.Count; i++)
                        {
                            var itemTracker = this.CreateItemTracker(i);
                            this.itemTrackers[i] = itemTracker;
                        }

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        for (int i = e.OldStartingIndex; i < e.OldStartingIndex + e.OldItems.Count; i++)
                        {
                            this.itemTrackers.RemoveAt(i);
                        }

                        break;
                    case NotifyCollectionChangedAction.Replace:
                        this.itemTrackers.RemoveAt(e.NewStartingIndex);
                        this.itemTrackers.Insert(e.NewStartingIndex, this.CreateItemTracker(e.NewStartingIndex));
                        break;
                    case NotifyCollectionChangedAction.Move:
                        this.itemTrackers[e.OldStartingIndex] = this.CreateItemTracker(e.OldStartingIndex);
                        this.itemTrackers[e.NewStartingIndex] = this.CreateItemTracker(e.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        {
                            var xList = (IList)this.x;
                            var yList = (IList)this.y;
                            this.itemTrackers.Clear();
                            for (int i = 0; i < Math.Max(xList.Count, yList.Count); i++)
                            {
                                var itemTracker = this.CreateItemTracker(i);
                                this.itemTrackers[i] = itemTracker;
                            }

                            break;
                        }

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (this.IsDirty)
                {
                    this.parent.diff.Add(ItemDirtyTracker.IndexerProperty);
                }
                else
                {
                    this.parent.diff.Remove(ItemDirtyTracker.IndexerProperty);
                }

                this.parent.NotifyChanges(before);
            }

            private IDirtyTrackerNode CreateItemTracker(int index)
            {
                var xList = (IList)this.x;
                var xv = xList.Count > index
                             ? xList[index]
                             : null;

                var yList = (IList)this.y;
                var yv = yList.Count > index
                             ? yList[index]
                             : null;
                if (xv == null && yv == null)
                {
                    return NeverDirtyNode.For(ItemDirtyTracker.IndexerProperty);
                }

                if (xv == null || yv == null)
                {
                    return AlwaysDirtyNode.For(ItemDirtyTracker.IndexerProperty);
                }

                if (xv.GetType().IsImmutable())
                {
                    return EqualBy.PropertyValues(xv, yv, this.parent.Settings)
                               ? (IDirtyTrackerNode)NeverDirtyNode.For(ItemDirtyTracker.IndexerProperty)
                               : AlwaysDirtyNode.For(ItemDirtyTracker.IndexerProperty);
                }

                switch (this.parent.Settings.ReferenceHandling)
                {
                    case ReferenceHandling.Throw:
                        var message = $"{typeof(Track).Name} does not support tracking an item of type {xv.GetType().Name}.\r\n" +
                                      $" Specify {typeof(ReferenceHandling).Name} if you want to track a graph";
                        throw new NotSupportedException(message);
                    case ReferenceHandling.References:
                        return ReferenceEquals(xv, yv)
                                   ? (IDirtyTrackerNode)NeverDirtyNode.For(ItemDirtyTracker.IndexerProperty)
                                   : AlwaysDirtyNode.For(ItemDirtyTracker.IndexerProperty);
                    case ReferenceHandling.Structural:
                        if (ReferenceEquals(xv, yv))
                        {
                            return NeverDirtyNode.For(ItemDirtyTracker.IndexerProperty);
                        }

                        return new ItemDirtyTracker(
                            (INotifyPropertyChanged)xv,
                            (INotifyPropertyChanged)yv,
                            this.parent);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
