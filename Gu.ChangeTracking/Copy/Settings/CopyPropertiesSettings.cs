namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class CopyPropertiesSettings : CopySettings, IEqualByPropertiesSettings
    {
        private static readonly Dictionary<BindingFlagsAndReferenceHandling, CopyPropertiesSettings> Cache = new Dictionary<BindingFlagsAndReferenceHandling, CopyPropertiesSettings>();
        private readonly HashSet<PropertyInfo> ignoredProperties;

        public CopyPropertiesSettings(IEnumerable<PropertyInfo> ignoredProperties, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            : this(ignoredProperties, null, bindingFlags, referenceHandling)
        {
        }

        public CopyPropertiesSettings(
            IEnumerable<PropertyInfo> ignoredProperties,
            IReadOnlyList<SpecialCopyProperty> specialCopyProperties,
            BindingFlags bindingFlags,
            ReferenceHandling referenceHandling)
            : base(bindingFlags, referenceHandling)
        {
            this.ignoredProperties = ignoredProperties != null
                                         ? new HashSet<PropertyInfo>(ignoredProperties)
                                         : null;
            this.SpecialCopyProperties = specialCopyProperties;
        }

        public CopyPropertiesSettings(Type type, string[] ignoreProperties, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            : this(type?.GetIgnoreProperties(bindingFlags, ignoreProperties), bindingFlags, referenceHandling)
        {
        }

        public IEnumerable<PropertyInfo> IgnoredProperties => this.ignoredProperties ?? Enumerable.Empty<PropertyInfo>();

        public IReadOnlyList<SpecialCopyProperty> SpecialCopyProperties { get; }

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
            CopyPropertiesSettings settings;
            if (Cache.TryGetValue(key, out settings))
            {
                return settings;
            }

            settings = new CopyPropertiesSettings((IEnumerable<PropertyInfo>) null,null,bindingFlags, referenceHandling);
            Cache[key] = settings;
            return settings;
        }

        public bool IsIgnoringProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null || propertyInfo.GetIndexParameters()
                                                    .Length > 0)
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