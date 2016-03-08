namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public partial class ChangeTracker
    {
        private sealed class PropertiesChangeTrackers : ITracker
        {
            private readonly INotifyPropertyChanged source;
            private readonly PropertiesSettings settings;
            private readonly INode<ItemReference, ITracker> node;

            private PropertiesChangeTrackers(
                INotifyPropertyChanged source,
                PropertiesSettings settings,
                INode<ItemReference, ITracker> node)
            {
                this.source = source;
                this.settings = settings;
                this.node = node.AddChild(source, () => this);
                source.PropertyChanged += this.OnTrackedPropertyChanged;

                foreach (var propertyInfo in GetTrackProperties(source.GetType(), settings))
                {
                    var tracker = CreatePropertyTracker(source, propertyInfo, settings, node);
                    node.AddChild(new ItemReference(), )
                    if (propertyTrackers == null)
                    {
                        propertyTrackers = new DisposingMap<PropertyInfo, IDisposable>();
                    }

                    propertyTrackers.SetValue(propertyInfo, tracker);
                }
            }

            public event EventHandler Changed;

            public void Dispose()
            {
                this.source.PropertyChanged -= this.OnTrackedPropertyChanged;
                this.propertyTrackers?.Dispose();
            }

            internal static PropertiesChangeTrackers Create(
                INotifyPropertyChanged source,
                PropertiesSettings settings,
                INode<ItemReference, ITracker> parentNode)
            {
                if (source == null)
                {
                    return null;
                }

                var sourceType = source.GetType();
                if (settings.IsIgnoringDeclaringType(sourceType))
                {
                    return null;
                }

                Track.Verify.IsTrackableType(source.GetType(), settings);
                return new PropertiesChangeTrackers(source, settings, parentNode);
            }

            private static IEnumerable<PropertyInfo> GetTrackProperties(Type sourceType, IIgnoringProperties settings)
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

            private static PropertyChangeTracker CreatePropertyTracker(
                object source,
                PropertyInfo propertyInfo,
                PropertiesSettings settings)
            {
                if (!IsTrackProperty(propertyInfo, settings))
                {
                    return null;
                }

                var sv = propertyInfo.GetValue(source);
                if (sv == null)
                {
                    return null;
                }

                Track.Verify.IsTrackablePropertyValue(sv.GetType(), propertyInfo, settings);
                var notifyPropertyChanged = sv as INotifyPropertyChanged;
                return new PropertyChangeTracker(notifyPropertyChanged, propertyInfo, settings);
            }

            private void OnTrackedPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (string.IsNullOrEmpty(e.PropertyName))
                {
                    this.OnChanged();
                    this.Reset();
                    return;
                }

                var propertyInfo = sender.GetType().GetProperty(e.PropertyName, Constants.DefaultPropertyBindingFlags);

                if (this.settings.IsIgnoringProperty(propertyInfo))
                {
                    return;
                }

                if (IsTrackProperty(propertyInfo, this.settings))
                {
                    CreatePropertyTracker(this.source, propertyInfo, this.node);
                }

                this.OnChanged();
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

            public void ChildChanged(ITracker child)
            {
                this.OnChanged();
            }

            private void OnChanged()
            {
                this.Changed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
