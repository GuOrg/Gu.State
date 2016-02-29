namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class CopyPropertiesSettings : CopySettings, IEqualByPropertiesSettings
    {
        private static readonly ConcurrentDictionary<BindingFlagsAndReferenceHandling, CopyPropertiesSettings> Cache = new ConcurrentDictionary<BindingFlagsAndReferenceHandling, CopyPropertiesSettings>();
        private readonly HashSet<PropertyInfo> ignoredProperties;

        public CopyPropertiesSettings(
            IEnumerable<PropertyInfo> ignoredProperties,
            IEnumerable<Type> ignoredTypes,
            BindingFlags bindingFlags,
            ReferenceHandling referenceHandling)
            : this(ignoredProperties, null, ignoredTypes, bindingFlags, referenceHandling)
        {
        }

        public CopyPropertiesSettings(
            IEnumerable<PropertyInfo> ignoredProperties,
            IReadOnlyList<SpecialCopyProperty> specialCopyProperties,
            IEnumerable<Type> ignoredTypes,
            BindingFlags bindingFlags,
            ReferenceHandling referenceHandling)
            : base(bindingFlags, referenceHandling, ignoredTypes)
        {
            this.ignoredProperties = ignoredProperties != null
                                         ? new HashSet<PropertyInfo>(ignoredProperties)
                                         : null;
            this.SpecialCopyProperties = specialCopyProperties;
        }

        public IEnumerable<PropertyInfo> IgnoredProperties => this.ignoredProperties ?? Enumerable.Empty<PropertyInfo>();

        public IReadOnlyList<SpecialCopyProperty> SpecialCopyProperties { get; }

        public static CopyPropertiesSettings Create(Type type, string[] excludedProperties, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            var ignored = type.GetIgnoreProperties(bindingFlags, excludedProperties);
            if (ignored == null || ignored.Count == 0)
            {
                return GetOrCreate(bindingFlags, referenceHandling);
            }

            return new CopyPropertiesSettings(ignored, null, null, bindingFlags, referenceHandling);
        }

        public static CopyPropertiesSettings GetOrCreate(ReferenceHandling referenceHandling)
        {
            return GetOrCreate(Constants.DefaultPropertyBindingFlags, referenceHandling);
        }

        public static CopyPropertiesSettings GetOrCreate(BindingFlags bindingFlags)
        {
            return GetOrCreate(bindingFlags, ReferenceHandling.Throw);
        }

        public static CopyPropertiesSettings GetOrCreate(BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            var key = new BindingFlagsAndReferenceHandling(bindingFlags, referenceHandling);
            return Cache.GetOrAdd(key, x => new CopyPropertiesSettings((IEnumerable<PropertyInfo>)null, null, bindingFlags, referenceHandling));
        }

        public bool IsIgnoringProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null || this.IsIgnoringType(propertyInfo.DeclaringType))
            {
                return true;
            }

            return this.ignoredProperties?.Contains(propertyInfo) == true;
        }

        bool IEqualByPropertiesSettings.IsIgnoringProperty(PropertyInfo propertyInfo)
        {
            return this.IsIgnoringProperty(propertyInfo) || this.GetSpecialCopyProperty(propertyInfo) != null;
        }

        public SpecialCopyProperty GetSpecialCopyProperty(PropertyInfo propertyInfo)
        {
            return this.SpecialCopyProperties?.SingleOrDefault(x => x.Property == propertyInfo);
        }
    }
}