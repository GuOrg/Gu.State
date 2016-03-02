namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class DirtyTrackerSettings : IEqualByPropertiesSettings
    {
        private static readonly ConcurrentDictionary<BindingFlagsAndReferenceHandling, DirtyTrackerSettings> Cache = new ConcurrentDictionary<BindingFlagsAndReferenceHandling, DirtyTrackerSettings>();
        private readonly IgnoredTypes ignoredTypes;
        private readonly HashSet<PropertyInfo> ignoredProperties;

        public DirtyTrackerSettings(IEnumerable<PropertyInfo> ignoredProperties, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            this.ignoredProperties = ignoredProperties != null
                                         ? new HashSet<PropertyInfo>(ignoredProperties)
                                         : null;
            this.BindingFlags = bindingFlags;
            this.ReferenceHandling = referenceHandling;
            this.ignoredTypes = IgnoredTypes.Create(null);
        }

        public IEnumerable<PropertyInfo> IgnoredProperties => this.ignoredProperties ?? Enumerable.Empty<PropertyInfo>();

        public BindingFlags BindingFlags { get; }

        public ReferenceHandling ReferenceHandling { get; }

        public static DirtyTrackerSettings Create<T>(T x, T y,string[] ignoreProperties,BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
            return Create(type, ignoreProperties, bindingFlags, referenceHandling);
        }

        public static DirtyTrackerSettings Create(Type type, string[] ignoreProperties, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            var ignored = type.GetIgnoreProperties(bindingFlags, ignoreProperties);
            if (ignored == null || ignored.Count == 0)
            {
                return GetOrCreate(bindingFlags, referenceHandling);
            }

            return new DirtyTrackerSettings(ignored, bindingFlags, referenceHandling);
        }

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
            return Cache.GetOrAdd(key, x => new DirtyTrackerSettings(null, bindingFlags, referenceHandling));
        }

        public bool IsIgnoringProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                return true;
            }

            if (this.ignoredTypes.IsIgnoringType(propertyInfo.DeclaringType))
            {
                return true;
            }

            if (this.ignoredProperties == null)
            {
                return false;
            }

            return this.ignoredProperties.Contains(propertyInfo);
        }

        public bool IsIgnoringDeclaringType(Type declaringType)
        {
            return this.ignoredTypes.IsIgnoringType(declaringType);
        }
    }
}
