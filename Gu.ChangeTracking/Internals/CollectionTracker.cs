namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reflection;

    internal sealed class CollectionTracker : PropertyTracker
    {
        private readonly ChangeTrackerSettings settings;
        private readonly CollectionItemTrackerCollection itemTrackers;

        public CollectionTracker(Type parentType, PropertyInfo parentProperty, IEnumerable value, ChangeTrackerSettings settings)
            : base(parentType, parentProperty, value)
        {
            this.settings = settings;
            this.itemTrackers = new CollectionItemTrackerCollection(parentType, parentProperty, settings);
            this.itemTrackers.PropertyChanged += OnSubtrackerPropertyChanged;
            var incc = value as INotifyCollectionChanged;
            if (incc != null)
            {
                incc.CollectionChanged += OnItemsChanged;
            }
            this.itemTrackers.Add(value);
        }

        private IEnumerable Items => (IEnumerable)Value;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var incc = Value as INotifyCollectionChanged;
                if (incc != null)
                {
                    incc.CollectionChanged -= OnItemsChanged;
                }

                this.itemTrackers.Dispose();
                this.itemTrackers.PropertyChanged -= OnSubtrackerPropertyChanged;
            }
            base.Dispose(disposing);
        }

        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Changes++;
            var type = sender.GetType();
            if (type.IsEnumerableOfT())
            {
                var itemType = type.GetItemType();
                if (itemType != null && !IsTrackType(itemType, this.settings))
                {
                    return;
                }
            }
            this.itemTrackers.Clear(); // keeping it simple here.
            this.itemTrackers.Add((IEnumerable)sender);
        }

        private void OnSubtrackerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Changes))
            {
                Changes++;
            }
        }
    }
}