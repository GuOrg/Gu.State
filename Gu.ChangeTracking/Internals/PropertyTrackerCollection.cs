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
    internal sealed class PropertyTrackerCollection : ChangeTracker, IReadOnlyCollection<IPropertyTracker>
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
            this.VerifyDisposed();
            return this.trackers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal void Add(INotifyPropertyChanged item, IReadOnlyList<PropertyInfo> trackProperties)
        {
            Ensure.NotNull(item, nameof(item));
            Ensure.NotNull(trackProperties, nameof(trackProperties));
            foreach (var property in trackProperties)
            {
                this.Add(item, property);
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
                this.Add(tracker);
            }
        }

        internal void Clear()
        {
            this.VerifyDisposed();
            this.ClearCore();
        }

        internal void RemoveBy(PropertyInfo propertyInfo)
        {
            var toRemove = this.trackers.SingleOrDefault(x => x.ParentProperty == propertyInfo);
            if (toRemove != null)
            {
                this.Remove(toRemove);
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
                this.ClearCore();
            }

            base.Dispose(disposing);
        }

        private void Add(IPropertyTracker tracker)
        {
            this.VerifyDisposed();
            if (tracker == null)
            {
                return;
            }

            tracker.PropertyChanged += this.OnItemPropertyChanged;
            var old = this.trackers.SingleOrDefault(x => x.ParentProperty.Name == tracker.ParentProperty.Name);
            if (old != null)
            {
                var message = $"Cannot have two trackers for the same property: {tracker.ParentType.Name}.{tracker.ParentProperty.Name} of the same instance: {tracker.Value}.\r\n" +
                              $" Remove old before adding new";
                throw new InvalidOperationException(message);
            }

            this.trackers.Add(tracker);
        }

        private void Remove(IPropertyTracker item)
        {
            this.VerifyDisposed();
            var removed = this.trackers.Remove(item);
            if (removed)
            {
                item.PropertyChanged -= this.OnItemPropertyChanged;
                item.Dispose();
            }
        }

        private void ClearCore()
        {
            foreach (var tracker in this.trackers)
            {
                if (tracker != null)
                {
                    tracker.Dispose();
                    tracker.PropertyChanged -= this.OnItemPropertyChanged;
                }
            }

            this.trackers.Clear();
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(this.Changes))
            {
                return;
            }

            this.Changes++;
        }
    }
}