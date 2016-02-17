namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// This class tracks property changes in source and keeps target in sync
    /// </summary>
    public class PropertySynchronizer<T> : IDisposable
        where T : class, INotifyPropertyChanged
    {
        private readonly T source;
        private readonly T target;

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
            : this(source, target, new CopyPropertiesSettings(source?.GetType().GetIgnoreProperties(bindingFlags, ignoreProperties), bindingFlags, ReferenceHandling.Throw))
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
            this.source = source;
            this.target = target;
            var allProperties = source.GetType().GetProperties(settings.BindingFlags);
            this.IgnoredProperties = allProperties.Where(settings.IsIgnoringProperty).ToArray();
            this.TrackedProperties = allProperties.Except(this.IgnoredProperties).ToArray();
            this.source.PropertyChanged += this.OnSourcePropertyChanged;
            Copy.PropertyValues(source, target, settings);
        }

        public CopyPropertiesSettings Settings { get; }

        public IReadOnlyList<PropertyInfo> TrackedProperties { get; }

        public IReadOnlyList<PropertyInfo> IgnoredProperties { get; }

        public void Dispose()
        {
            this.source.PropertyChanged -= this.OnSourcePropertyChanged;
        }

        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IgnoredProperties.Any(p => p.Name == e.PropertyName))
            {
                return;
            }

            if (string.IsNullOrEmpty(e.PropertyName))
            {
                Copy.WritableProperties(this.source, this.target, this.TrackedProperties, this.Settings);
                Copy.VerifyReadonlyPropertiesAreEqual(this.source, this.target, this.TrackedProperties, this.Settings);
                return;
            }

            var propertyInfo = this.source.GetType().GetProperty(e.PropertyName, this.Settings.BindingFlags);
            Copy.PropertyValue(this.source, this.target, propertyInfo);
        }
    }
}