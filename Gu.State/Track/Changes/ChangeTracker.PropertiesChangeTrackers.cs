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
            private readonly INode<ITracker> node;

            private PropertiesChangeTrackers(
                INotifyPropertyChanged source,
                PropertiesSettings settings,
                INode<ITracker> node)
            {
                this.source = source;
                this.settings = settings;
                this.node = node.AddChild(source, () => this);
                source.PropertyChanged += this.OnTrackedPropertyChanged;

                foreach (var propertyInfo in GetTrackProperties(source.GetType(), settings))
                {
                    CreatePropertyTracker(source, propertyInfo, settings, node);
                }
            }

            public event EventHandler Changed;

            public void Dispose()
            {
                this.source.PropertyChanged -= this.OnTrackedPropertyChanged;
                this.node.Dispose();
            }

            internal static PropertiesChangeTrackers Create(
                INotifyPropertyChanged source,
                PropertiesSettings settings,
                INode<ITracker> parentNode)
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
                PropertiesSettings settings,
                INode<ITracker> node)
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
                    CreatePropertyTracker(this.source, propertyInfo, this.settings, this.node);
                }

                this.OnChanged();
            }

            private void Reset()
            {
                foreach (var propertyInfo in GetTrackProperties(this.source?.GetType(), this.settings))
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
