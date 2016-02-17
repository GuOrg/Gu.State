namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;

    /// <summary>
    /// This class tracks property changes in source and keeps target in sync
    /// </summary>
    public class PropertySynchronizer<T> : IDisposable
        where T : class, INotifyPropertyChanged
    {
        private readonly T source;
        private readonly T target;
        private readonly Lazy<Dictionary<object, PropertySynchronizer<INotifyPropertyChanged>>> subPropertySynchronizers =
            new Lazy<Dictionary<object, PropertySynchronizer<INotifyPropertyChanged>>>(
                () => new Dictionary<object, PropertySynchronizer<INotifyPropertyChanged>>());

        public PropertySynchronizer(T source, T target, ReferenceHandling referenceHandling)
            : this(source, target, Constants.DefaultPropertyBindingFlags, referenceHandling)
        {
        }

        public PropertySynchronizer(T source, T target, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            : this(source, target, new CopyPropertiesSettings(null, bindingFlags, referenceHandling))
        {
        }

        public PropertySynchronizer(T source, T target, params string[] ignoreProperties)
            : this(source, target, Constants.DefaultPropertyBindingFlags, ignoreProperties)
        {
        }

        public PropertySynchronizer(T source, T target, BindingFlags bindingFlags, params string[] ignoreProperties)
            : this(source,
                target,
                new CopyPropertiesSettings(
                    source?.GetType().GetIgnoreProperties(bindingFlags, ignoreProperties),
                    bindingFlags,
                    ReferenceHandling.Throw))
        {
        }

        public PropertySynchronizer(T source, T target, CopyPropertiesSettings settings)
        {
            Ensure.NotSame(source, target, nameof(source), nameof(target));
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));
            Ensure.SameType(source, target, nameof(source), nameof(target));
            Copy.VerifyCanCopyPropertyValues<T>(settings);
            this.Settings = settings;
            this.target = target;
            this.source = source;
            this.source.PropertyChanged += this.OnSourcePropertyChanged;
            Copy.PropertyValues(source, target, settings);
            foreach (var propertyInfo in this.source.GetType().GetProperties(this.Settings.BindingFlags))
            {
                if (this.Settings.IsIgnoringProperty(propertyInfo) ||
                    this.Settings.GetSpecialCopyProperty(propertyInfo) != null)
                {
                    continue;
                }

                if (!Copy.IsCopyableType(propertyInfo.PropertyType))
                {
                    this.UpdateSubPropertySynchronizer(propertyInfo);
                }
            }
        }

        public CopyPropertiesSettings Settings { get; }

        public void Dispose()
        {
            this.source.PropertyChanged -= this.OnSourcePropertyChanged;
            if (this.subPropertySynchronizers.IsValueCreated)
            {
                foreach (var propertySynchronizer in this.subPropertySynchronizers.Value)
                {
                    propertySynchronizer.Value.Dispose();
                }
            }
        }

        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                Copy.PropertyValues(this.source, this.target, this.Settings);
                return;
            }

            var propertyInfo = this.source.GetType()
                                   .GetProperty(e.PropertyName, this.Settings.BindingFlags);
            if (propertyInfo == null)
            {
                return;
            }

            if (this.Settings.IsIgnoringProperty(propertyInfo))
            {
                return;
            }

            var specialCopyProperty = this.Settings.GetSpecialCopyProperty(propertyInfo);
            if (specialCopyProperty != null)
            {
                specialCopyProperty.CopyValue(this.source, this.target);
                return;
            }

            if (!Copy.IsCopyableType(propertyInfo.PropertyType))
            {
                Copy.PropertyValues(this.source, this.target, this.Settings);
                this.UpdateSubPropertySynchronizer(propertyInfo);
            }
            else
            {
                Copy.PropertyValue(this.source, this.target, propertyInfo);
            }
        }

        private void UpdateSubPropertySynchronizer(PropertyInfo propertyInfo)
        {
            var propertySynchronizers = this.subPropertySynchronizers.Value;
            PropertySynchronizer<INotifyPropertyChanged> synchronizer;
            var sv = (INotifyPropertyChanged)propertyInfo.GetValue(this.source);
            var tv = (INotifyPropertyChanged)propertyInfo.GetValue(this.target);

            if (propertySynchronizers.TryGetValue(propertyInfo, out synchronizer))
            {
                synchronizer.Dispose();
            }
            if (sv != null)
            {
                synchronizer = new PropertySynchronizer<INotifyPropertyChanged>(sv, tv, this.Settings);
                propertySynchronizers[propertyInfo] = synchronizer;
            }
        }
    }
}