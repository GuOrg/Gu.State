namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    internal sealed class PropertyChangeTracker : PropertyTracker
    {
        private readonly ChangeTrackerSettings settings;
        private readonly PropertyTrackerCollection propertyTrackers;
        private HashSet<string> ignoredProperties;

        internal PropertyChangeTracker(Type parentType, PropertyInfo parentProperty, INotifyPropertyChanged value, ChangeTrackerSettings settings)
            : base(parentType, parentProperty, value)
        {
            Ensure.NotNull(parentType, nameof(parentType));
            Ensure.NotNull(parentProperty, nameof(parentProperty));
            Ensure.NotNull(value, nameof(value));
            Ensure.NotNull(settings, nameof(settings));

            this.settings = settings;
            this.propertyTrackers = new PropertyTrackerCollection(value.GetType(), settings);
            value.PropertyChanged += OnItemPropertyChanged;
            this.propertyTrackers.PropertyChanged += OnSubtrackerPropertyChanged;
            this.propertyTrackers.Add(value, TrackProperties);
            this.ignoredProperties = new HashSet<string>();
            foreach (var property in value.GetType().GetProperties())
            {
                if(Attribute.GetCustomAttribute(property, typeof(IgnoreChangesAttribute)) != null ||
                   settings.IsIgnored(property))
                {
                    this.ignoredProperties.Add(property.Name);
                }
            }
        }

        private new INotifyPropertyChanged Value => (INotifyPropertyChanged)base.Value;

        private IReadOnlyList<PropertyInfo> TrackProperties => GetTrackProperties(Value, this.settings);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Value.PropertyChanged -= OnItemPropertyChanged;
                this.propertyTrackers.Dispose();
            }

            base.Dispose(disposing);
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.ignoredProperties.Contains(e.PropertyName))
            {
                return;
            }

            Changes++;
            var propertyInfo = TrackProperties.SingleOrDefault(x => x.Name == e.PropertyName);
            if (propertyInfo != null)
            {
                this.propertyTrackers.RemoveBy(propertyInfo);
                this.propertyTrackers.Add((INotifyPropertyChanged)sender, propertyInfo);
            }
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
