namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reflection;

    public partial class PropertySynchronizer<T>
    {
        private class PropertiesSynchronizer : IDisposable
        {
            private readonly INotifyPropertyChanged source;
            private readonly INotifyPropertyChanged target;
            private readonly PropertiesSettings settings;
            private readonly TwoItemsTrackerReferenceCollection<IPropertySynchronizer> references;
            private readonly PropertyCollection propertySynchronizers;

            private PropertiesSynchronizer(
                INotifyPropertyChanged source,
                INotifyPropertyChanged target,
                PropertyCollection propertySynchronizers,
                PropertiesSettings settings,
                TwoItemsTrackerReferenceCollection<IPropertySynchronizer> references)
            {
                source.PropertyChanged += this.OnSourcePropertyChanged;
                this.source = source;
                this.target = target;
                this.settings = settings;
                this.references = references;
                this.propertySynchronizers = propertySynchronizers;
            }

            public void Dispose()
            {
                this.source.PropertyChanged -= this.OnSourcePropertyChanged;
                this.propertySynchronizers?.Dispose();
            }

            internal static PropertiesSynchronizer Create(
                INotifyPropertyChanged source,
                INotifyPropertyChanged target,
                PropertiesSettings settings,
                TwoItemsTrackerReferenceCollection<IPropertySynchronizer> references)
            {
                if (source == null)
                {
                    return null;
                }

                List<PropertyCollection.PropertyAndDisposable> items = null;
                foreach (var propertyInfo in source.GetType()
                                                   .GetProperties(settings.BindingFlags))
                {
                    if (settings.IsIgnoringProperty(propertyInfo) ||
                        settings.GetSpecialCopyProperty(propertyInfo) != null)
                    {
                        continue;
                    }

                    if (!Copy.IsCopyableType(propertyInfo.PropertyType))
                    {
                        var sv = propertyInfo.GetValue(source);
                        var tv = propertyInfo.GetValue(target);
                        var synchronizer = CreateSynchronizer((INotifyPropertyChanged)sv, (INotifyPropertyChanged)tv, settings, references);
                        if (items == null)
                        {
                            items = new List<PropertyCollection.PropertyAndDisposable>();
                        }

                        items.Add(new PropertyCollection.PropertyAndDisposable(propertyInfo, synchronizer));
                    }
                }

                if (items == null)
                {
                    return new PropertiesSynchronizer(source, target, null, settings, references);
                }

                var propertyCollection = new PropertyCollection(items);
                return new PropertiesSynchronizer(source, target, propertyCollection, settings, references);
            }

            private static IDisposable CreateSynchronizer(object sv, object tv, PropertiesSettings settings, TwoItemsTrackerReferenceCollection<IPropertySynchronizer> references)
            {
                if (sv == null || Copy.IsCopyableType(sv.GetType()))
                {
                    return null;
                }

                if (settings.ReferenceHandling == ReferenceHandling.Throw)
                {
                    throw new NotSupportedException("Specify how to handle reference types using ReferenceHandling");
                }

                if (ReferenceEquals(sv, tv))
                {
                    return null;
                }

                if (references != null)
                {
                    return references.GetOrAdd(
                        sv,
                        tv,
                        () =>
                        new PropertySynchronizer<INotifyPropertyChanged>(
                            (INotifyPropertyChanged)sv,
                            (INotifyPropertyChanged)tv,
                            settings,
                            references));
                }

                return new PropertySynchronizer<INotifyPropertyChanged>((INotifyPropertyChanged)sv, (INotifyPropertyChanged)tv, settings, references);
            }

            private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (string.IsNullOrEmpty(e.PropertyName))
                {
                    Copy.PropertyValues(this.source, this.target, this.settings);
                    return;
                }

                var propertyInfo = this.source.GetType().GetProperty(e.PropertyName, this.settings.BindingFlags);
                if (propertyInfo == null)
                {
                    return;
                }

                if (this.settings.IsIgnoringProperty(propertyInfo) ||
                    (this.source is INotifyCollectionChanged && e.PropertyName == "Count"))
                {
                    return;
                }

                var specialCopyProperty = this.settings.GetSpecialCopyProperty(propertyInfo);
                if (specialCopyProperty != null)
                {
                    specialCopyProperty.CopyValue(this.source, this.target);
                    return;
                }

                if (!Copy.IsCopyableType(propertyInfo.PropertyType))
                {
                    Copy.PropertyValues(this.source, this.target, this.settings);
                    this.UpdateSubPropertySynchronizer(propertyInfo);
                }
                else
                {
                    Copy.PropertyValue(this.source, this.target, propertyInfo);
                }
            }

            private void UpdateSubPropertySynchronizer(PropertyInfo propertyInfo)
            {
                var sv = (INotifyPropertyChanged)propertyInfo.GetValue(this.source);
                var tv = (INotifyPropertyChanged)propertyInfo.GetValue(this.target);
                this.propertySynchronizers[propertyInfo] = CreateSynchronizer(sv, tv, this.settings, this.references);
            }
        }

    }
}
