namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class EqualByPropertiesSettings : EqualBySettings, IEqualByPropertiesSettings
    {
        private readonly HashSet<PropertyInfo> ignoredProperties;
        private static readonly ConcurrentDictionary<BindingFlagsAndReferenceHandling, EqualByPropertiesSettings> Cache = new ConcurrentDictionary<BindingFlagsAndReferenceHandling, EqualByPropertiesSettings>();

        public EqualByPropertiesSettings(Type type, string[] ignoredProperties, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            : this(type?.GetIgnoreProperties(bindingFlags, ignoredProperties), bindingFlags, referenceHandling)
        {
        }

        public EqualByPropertiesSettings(IEnumerable<PropertyInfo> ignoredProperties, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            : base(bindingFlags, referenceHandling, null)
        {
            this.ignoredProperties = ignoredProperties != null
                                         ? new HashSet<PropertyInfo>(ignoredProperties)
                                         : null;
        }

        public IEnumerable<PropertyInfo> IgnoredProperties => this.ignoredProperties ?? Enumerable.Empty<PropertyInfo>();

        public static EqualByPropertiesSettings Create<T>(T x, T y, BindingFlags bindingFlags, ReferenceHandling referenceHandling, string[] excludedProperties)
        {
            var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
            return Create(type, bindingFlags, referenceHandling, excludedProperties);
        }

        public static EqualByPropertiesSettings Create(Type type, BindingFlags bindingFlags, ReferenceHandling referenceHandling, string[] excludedProperties)
        {
            var ignored = type.GetIgnoreProperties(bindingFlags, excludedProperties);
            if (ignored == null || ignored.Count == 0)
            {
                return GetOrCreate(bindingFlags, referenceHandling);
            }

            return new EqualByPropertiesSettings(ignored, bindingFlags, referenceHandling);
        }

        public static EqualByPropertiesSettings GetOrCreate(ReferenceHandling referenceHandling)
        {
            return GetOrCreate(Constants.DefaultPropertyBindingFlags, referenceHandling);
        }

        public static EqualByPropertiesSettings GetOrCreate(BindingFlags bindingFlags)
        {
            return GetOrCreate(bindingFlags, ReferenceHandling.Throw);
        }

        public static EqualByPropertiesSettings GetOrCreate(BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            var key = new BindingFlagsAndReferenceHandling(bindingFlags, referenceHandling);
            return Cache.GetOrAdd(key, x => new EqualByPropertiesSettings(null, bindingFlags, referenceHandling));
        }

        public bool IsIgnoringProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null || propertyInfo.GetIndexParameters().Length > 0)
            {
                return true;
            }

            if (this.IsIgnoringType(propertyInfo.DeclaringType))
            {
                return true;
            }

            if (this.ignoredProperties == null)
            {
                return false;
            }

            return this.ignoredProperties.Contains(propertyInfo);
        }
    }
}