namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class EqualByFieldsSettings : EqualBySettings
    {
        private static readonly Dictionary<BindingFlagsAndReferenceHandling, EqualByFieldsSettings> Cache = new Dictionary<BindingFlagsAndReferenceHandling, EqualByFieldsSettings>();
        private readonly HashSet<FieldInfo> ignoredFields;

        public EqualByFieldsSettings(Type type, string[] ignoredProperties, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            : this(type?.GetIgnoreFields(bindingFlags, ignoredProperties), bindingFlags, referenceHandling)
        {
        }

        public EqualByFieldsSettings(IEnumerable<FieldInfo> ignoredFields, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            : base(bindingFlags, referenceHandling)
        {
            this.ignoredFields = ignoredFields != null
                                     ? new HashSet<FieldInfo>(ignoredFields)
                                     : null;
        }

        public IEnumerable<FieldInfo> IgnoredFields => this.ignoredFields ?? Enumerable.Empty<FieldInfo>();

        public static EqualByFieldsSettings Create<T>(T x, T y, BindingFlags bindingFlags, ReferenceHandling referenceHandling, params string[] excludedFields)
        {
            var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
            var ignoreFields = type.GetIgnoreFields(bindingFlags, excludedFields);
            if (ignoreFields == null || ignoreFields.Count == 0)
            {
                return GetOrCreate(bindingFlags, referenceHandling);
            }

            return new EqualByFieldsSettings(ignoreFields, bindingFlags, referenceHandling);
        }

        public static EqualByFieldsSettings GetOrCreate(BindingFlags bindingFlags)
        {
            return GetOrCreate(bindingFlags, ReferenceHandling.Throw);
        }

        public static EqualByFieldsSettings GetOrCreate(ReferenceHandling referenceHandling)
        {
            return GetOrCreate(Constants.DefaultFieldBindingFlags, referenceHandling);
        }

        public static EqualByFieldsSettings GetOrCreate(BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            var key = new BindingFlagsAndReferenceHandling(bindingFlags, referenceHandling);
            EqualByFieldsSettings settings;
            if (Cache.TryGetValue(key, out settings))
            {
                return settings;
            }

            settings = new EqualByFieldsSettings(null, bindingFlags, referenceHandling);
            Cache[key] = settings;
            return settings;
        }

        public bool IsIgnoringField(FieldInfo fieldInfo)
        {
            if (fieldInfo == null || fieldInfo.IsEventField())
            {
                return true;
            }

            if (this.ignoredFields == null)
            {
                return false;
            }

            return this.ignoredFields.Contains(fieldInfo);
        }
    }
}