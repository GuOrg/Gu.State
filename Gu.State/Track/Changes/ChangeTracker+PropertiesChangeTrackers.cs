namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public partial class ChangeTracker
    {
        private sealed class PropertiesChangeTrackers : IDisposable
        {
            private readonly INotifyPropertyChanged source;
            private readonly ChangeTracker parent;
            private readonly DisposingMap<PropertyInfo, IDisposable> propertyTrackers;

            private PropertiesChangeTrackers(INotifyPropertyChanged source, ChangeTracker parent, DisposingMap<PropertyInfo, IDisposable> propertyTrackers)
            {
                this.source = source;
                this.parent = parent;
                this.propertyTrackers = propertyTrackers;
                source.PropertyChanged += this.OnTrackedPropertyChanged;
            }

            public void Dispose()
            {
                this.source.PropertyChanged -= this.OnTrackedPropertyChanged;
                this.propertyTrackers?.Dispose();
            }

            internal static PropertiesChangeTrackers Create(INotifyPropertyChanged source, ChangeTracker parent)
            {
                if (source == null)
                {
                    return null;
                }

                var sourceType = source.GetType();
                if (parent.Settings.IsIgnoringDeclaringType(sourceType))
                {
                    return null;
                }

                Track.Verify.IsTrackableType(source.GetType(), parent);
                DisposingMap<PropertyInfo, IDisposable> items = null;
                foreach (var propertyInfo in GetTrackProperties(sourceType, parent.Settings))
                {
                    var tracker = CreatePropertyTracker(source, propertyInfo, parent);
                    if (items == null)
                    {
                        items = new DisposingMap<PropertyInfo, IDisposable>();
                    }

                    items.SetValue(propertyInfo, tracker);
                }

                return new PropertiesChangeTrackers(source, parent, items);
            }

            internal static IEnumerable<PropertyInfo> GetTrackProperties(Type sourceType, IIgnoringProperties settings)
            {
                return sourceType.GetProperties(Constants.DefaultPropertyBindingFlags)
                                 .Where(p => IsTrackProperty(p, settings));
            }

            private static bool IsTrackProperty(PropertyInfo propertyInfo, IIgnoringProperties settings)
            {
                if (settings.IsIgnoringProperty(propertyInfo))
                {
                    return false;
                }

                if (propertyInfo.PropertyType.IsImmutable())
                {
                    return false;
                }

                return true;
            }

            private static PropertyChangeTracker CreatePropertyTracker(object source, PropertyInfo propertyInfo, ChangeTracker parent)
            {
                if (!IsTrackProperty(propertyInfo, parent.Settings))
                {
                    return null;
                }

                var sv = propertyInfo.GetValue(source);
                if (sv == null)
                {
                    return null;
                }

                Track.Verify.IsTrackablePropertyValue(sv.GetType(), propertyInfo, parent);
                var notifyPropertyChanged = sv as INotifyPropertyChanged;
                return new PropertyChangeTracker(notifyPropertyChanged, propertyInfo, parent);
            }

            private void OnTrackedPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (string.IsNullOrEmpty(e.PropertyName))
                {
                    this.parent.Changes++;
                    this.Reset();
                    return;
                }

                var propertyInfo = sender.GetType().GetProperty(e.PropertyName, Constants.DefaultPropertyBindingFlags);

                if (this.parent.Settings.IsIgnoringProperty(propertyInfo))
                {
                    return;
                }

                if (IsTrackProperty(propertyInfo, this.parent.Settings))
                {
                    var tracker = CreatePropertyTracker(this.source, propertyInfo, this.parent);
                    this.propertyTrackers.SetValue(propertyInfo, tracker);
                }

                this.parent.Changes++;
            }

            private void Reset()
            {
                if (this.propertyTrackers == null)
                {
                    return;
                }

                foreach (var propertyInfo in GetTrackProperties(this.source?.GetType(), this.parent.Settings))
                {
                    // might be worth it to check if Source ReferenceEquals to avoid creating a new tracker here.
                    // Probably not a big problem as I expect PropertyChanged.Invoke(string.Empty) to be rare.
                    var tracker = CreatePropertyTracker(this.source, propertyInfo, this.parent);
                    this.propertyTrackers.SetValue(propertyInfo, tracker);
                }
            }
        }
    }
}
