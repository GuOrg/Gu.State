namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    [DebuggerDisplay("Count: {Count}")]
    internal sealed class PropertyTrackerCollection : ChangeTracker, IReadOnlyCollection<IPropertyTracker>, IDisposable
    {
        private readonly Type parentType;
        private readonly ChangeTrackerSettings settings;
        private readonly List<IPropertyTracker> trackers = new List<IPropertyTracker>();

        public PropertyTrackerCollection(Type parentType, ChangeTrackerSettings settings)
        {
            this.parentType = parentType;
            this.settings = settings;
        }

        public int Count => this.trackers.Count;

        public bool IsReadOnly => false;

        public IEnumerator<IPropertyTracker> GetEnumerator()
        {
            VerifyDisposed();
            return this.trackers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void Add(INotifyPropertyChanged item, IReadOnlyList<PropertyInfo> trackProperties)
        {
            Ensure.NotNull(item, nameof(item));
            Ensure.NotNull(trackProperties, nameof(trackProperties));
            foreach (var property in trackProperties)
            {
                Add(item, property);
            }
        }

        internal void Add(INotifyPropertyChanged item, PropertyInfo property)
        {
            Ensure.NotNull(item, nameof(item));
            Ensure.NotNull(property, nameof(property));
            var value = property.GetValue(item);
            if (!CanTrack(this.parentType, property, value, this.settings))
            {
                return;
            }
            if (value == null)
            {
                return;
            }
            var tracker = Create(this.parentType, property, value, this.settings);
            if (tracker != null)
            {
                Add(tracker);
            }
        }

        internal void Clear()
        {
            VerifyDisposed();
            ClearCore();
        }

        internal void RemoveBy(PropertyInfo propertyInfo)
        {
            var toRemove = this.trackers.SingleOrDefault(x => x.ParentProperty == propertyInfo);
            if (toRemove != null)
            {
                Remove(toRemove);
            }
        }

        /// <summary>
        /// Make the class sealed when using this. 
        /// Call VerifyDisposed at the start of all public methods
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearCore();
            }

            base.Dispose(disposing);
        }

        private void Add(IPropertyTracker tracker)
        {
            VerifyDisposed();
            if (tracker == null)
            {
                return;
            }
            tracker.PropertyChanged += OnItemPropertyChanged;
            var old = this.trackers.SingleOrDefault(x => x.ParentProperty.Name == tracker.ParentProperty.Name);
            if (old != null)
            {
                var message =
                    string.Format(
                        "Cannot have two trackers for the same property: {0}.{1} of the same instance: {2}. Remove old before adding new",
                        tracker.ParentType.Name,
                        tracker.ParentProperty.Name,
                        tracker.Value);
                throw new InvalidOperationException(message);
            }

            this.trackers.Add(tracker);
        }

        private bool Remove(IPropertyTracker item)
        {
            VerifyDisposed();
            var removed = this.trackers.Remove(item);
            if (removed)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
                item.Dispose();
            }

            return removed;
        }

        private void ClearCore()
        {
            foreach (var tracker in this.trackers)
            {
                if (tracker != null)
                {
                    tracker.Dispose();
                    tracker.PropertyChanged -= OnItemPropertyChanged;
                }
            }
            this.trackers.Clear();
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Changes))
            {
                return;
            }

            Changes++;
        }
    }
}