namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    internal sealed class CollectionItemTrackerCollection : ChangeTracker, IReadOnlyCollection<IValueTracker>
    {
        private readonly Type parentType;
        private readonly PropertyInfo parentProperty;
        private readonly ChangeTrackerSettings settings;
        private readonly List<IValueTracker> trackers = new List<IValueTracker>();

        public CollectionItemTrackerCollection(Type parentType, PropertyInfo parentProperty, ChangeTrackerSettings settings)
        {
            Ensure.NotNull(parentType, nameof(parentType));
            Ensure.NotNull(parentProperty, nameof(parentProperty));
            this.parentType = parentType;
            this.parentProperty = parentProperty;
            this.settings = settings;
        }

        /// <inheritdoc/>
        public int Count => this.trackers.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => true;

        /// <inheritdoc/>
        public IEnumerator<IValueTracker> GetEnumerator() => this.trackers.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <see cref="List{IValueTracker}.Contains(IValueTracker)"/>
        public bool Contains(IValueTracker item)
        {
            this.VerifyDisposed();
            return this.trackers.Contains(item);
        }

        /// <see cref="List{IValueTracker}.Add(IValueTracker)"/>
        internal void Add(IEnumerable items)
        {
            foreach (var child in items)
            {
                var itemTracker = Create(this.parentType, this.parentProperty, child, this.settings);
                if (itemTracker != null)
                {
                    this.Add(itemTracker);
                }
            }
        }

        /// <see cref="List{IValueTracker}.Clear()"/>
        internal void Clear()
        {
            this.VerifyDisposed();
            this.ClearCore();
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

        private void Add(IValueTracker item)
        {
            var match = this.trackers.FirstOrDefault(x => ReferenceEquals(x.Value, item));
            if (match != null)
            {
                throw new InvalidOperationException("Cannot track the same item twice. Clear before add");
            }

            item.PropertyChanged += this.OnItemPropertyChanged;
            this.trackers.Add(item);
        }
    }
}
