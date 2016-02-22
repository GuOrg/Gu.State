namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class DirtyTrackerSettings
    {
        private static readonly Dictionary<BindingFlagsAndReferenceHandling, DirtyTrackerSettings> Cache = new Dictionary<BindingFlagsAndReferenceHandling, DirtyTrackerSettings>();
        private readonly HashSet<PropertyInfo> ignoredProperties;

        public DirtyTrackerSettings(Type type, string[] ignoreProperties, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            : this(type?.GetIgnoreProperties(bindingFlags, ignoreProperties), bindingFlags, referenceHandling)
        {
        }

        public DirtyTrackerSettings(IEnumerable<PropertyInfo> ignoredProperties, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            this.ignoredProperties = ignoredProperties != null
                                         ? new HashSet<PropertyInfo>(ignoredProperties)
                                         : null;
            this.BindingFlags = bindingFlags;
            this.ReferenceHandling = referenceHandling;
        }

        public IEnumerable<PropertyInfo> IgnoredProperties => this.ignoredProperties ?? Enumerable.Empty<PropertyInfo>();

        public BindingFlags BindingFlags { get; }

        public ReferenceHandling ReferenceHandling { get; }

        public static DirtyTrackerSettings GetOrCreate(ReferenceHandling referenceHandling)
        {
            return GetOrCreate(Constants.DefaultPropertyBindingFlags, referenceHandling);
        }

        public static DirtyTrackerSettings GetOrCreate(BindingFlags bindingFlags)
        {
            return GetOrCreate(bindingFlags, ReferenceHandling.Throw);
        }

        public static DirtyTrackerSettings GetOrCreate(BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            var key = new BindingFlagsAndReferenceHandling(bindingFlags, referenceHandling);
            DirtyTrackerSettings settings;
            if (Cache.TryGetValue(key, out settings))
            {
                return settings;
            }

            settings = new DirtyTrackerSettings(null, bindingFlags, referenceHandling);
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

            if (this.ignoredProperties == null)
            {
                return false;
            }

            return this.ignoredProperties.Contains(propertyInfo);
        }
    }
}
