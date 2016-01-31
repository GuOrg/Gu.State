namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// This class tracks propertu changes in source and keeps target in sync
    /// </summary>
    public class PropertySynchronizer<T> : IDisposable
        where T : class, INotifyPropertyChanged
    {
        private readonly T source;
        private readonly T target;

        public PropertySynchronizer(T source, T target, string[] ignoreProperties)
            : this(source, target, Constants.DefaultPropertyBindingFlags, ignoreProperties)
        {
        }

        public PropertySynchronizer(T source, T target, BindingFlags bindingFlags, string[] ignoreProperties)
        {
            Ensure.NotSame(source, target, nameof(source), nameof(target));
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));
            Copy.VerifyCanCopyPropertyValues<T>(ignoreProperties);
            this.BindingFlags = bindingFlags;
            this.source = source;
            this.target = target;
            var allProperties = typeof(T).GetProperties(bindingFlags);
            this.IgnoredProperties = allProperties.Where(p => ignoreProperties.Contains(p.Name)).ToArray();
            this.TrackedProperties = allProperties.Except(this.IgnoredProperties).ToArray();
            this.source.PropertyChanged += this.OnSourcePropertyChanged;
            Copy.PropertyValues(source, target, ignoreProperties);
        }

        public BindingFlags BindingFlags { get; }

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
                Copy.WritableProperties(this.source, this.target, this.TrackedProperties, null);
                Copy.VerifyReadonlyPropertiesAreEqual(this.source, this.target, this.TrackedProperties, null);
                return;
            }

            var propertyInfo = this.source.GetType().GetProperty(e.PropertyName, this.BindingFlags);
            Copy.PropertyValue(this.source, this.target, propertyInfo);
        }
    }
}