namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;

    public partial class ChangeTracker
    {
        private sealed class ItemsChangeTrackers : IDisposable
        {
            private readonly INotifyCollectionChanged source;
            private readonly ChangeTracker parent;
            private readonly DisposingList<ChangeTracker> itemTrackers;

            private ItemsChangeTrackers(INotifyCollectionChanged source, ChangeTracker parent, DisposingList<ChangeTracker> itemTrackers)
            {
                this.source = source;
                this.parent = parent;
                this.itemTrackers = itemTrackers;
                this.source.CollectionChanged += this.OnTrackedCollectionChanged;
            }

            public void Dispose()
            {
                this.source.CollectionChanged -= this.OnTrackedCollectionChanged;
                this.itemTrackers?.Dispose();
            }

            internal static ItemsChangeTrackers Create(object source, ChangeTracker parent)
            {
                if (!(source is IEnumerable))
                {
                    return null;
                }

                var sourceType = source.GetType();

                Track.Verify.IsTrackableType(sourceType, parent.Settings);

                var incc = source as INotifyCollectionChanged;
                var itemType = source.GetType().GetItemType();
                if (itemType.IsImmutable() || parent.Settings.IsIgnoringDeclaringType(itemType))
                {
                    return new ItemsChangeTrackers(incc, parent, null);
                }

                var sourceList = (IList)source;
                var itemTrackers = new DisposingList<ChangeTracker>(sourceList.Count);
                for (int i = 0; i < sourceList.Count; i++)
                {
                    var itemTracker = CreateItemTracker(sourceList, i, parent);
                    itemTrackers[i] = itemTracker;
                }

                return new ItemsChangeTrackers(incc, parent, itemTrackers);
            }

            private static ItemChangeTrackerOld CreateItemTracker(IList source, int index, ChangeTracker parent)
            {
                Debug.Assert(!source.GetType().GetItemType().IsImmutable(), "Creating a tracker for an immutable type would be wasteful");
                var sv = source[index];
                if (sv == null)
                {
                    return null;
                }

                var itemType = sv.GetType();
                Track.Verify.IsTrackableItemValue(source.GetType(), itemType, index, parent);
                var inpc = sv as INotifyPropertyChanged;
                return new ItemChangeTrackerOld(inpc, index, parent);
            }

            private void OnTrackedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (this.itemTrackers == null)
                {
                    this.parent.Changes++;
                    return;
                }

                var sourceList = (IList)this.source;
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        for (int i = e.NewStartingIndex; i < e.NewStartingIndex + e.NewItems.Count; i++)
                        {
                            var itemTracker = CreateItemTracker(sourceList, i, this.parent);
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
                        this.itemTrackers.Insert(e.NewStartingIndex, CreateItemTracker(sourceList, e.NewStartingIndex, this.parent));
                        break;
                    case NotifyCollectionChangedAction.Move:
                        this.itemTrackers[e.OldStartingIndex] = CreateItemTracker(sourceList, e.OldStartingIndex, this.parent);
                        this.itemTrackers[e.NewStartingIndex] = CreateItemTracker(sourceList, e.NewStartingIndex, this.parent);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        {
                            var xList = (IList)this.source;
                            this.itemTrackers.Clear();
                            for (int i = 0; i < xList.Count; i++)
                            {
                                var itemTracker = CreateItemTracker(sourceList, i, this.parent);
                                this.itemTrackers[i] = itemTracker;
                            }

                            break;
                        }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(e.Action));
                }

                this.parent.Changes++;
            }
        }
    }
}
