namespace Gu.ChangeTracking
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class EqualByFieldsSettings : EqualBySettings, IEqualByFieldsSettings
    {
        private static readonly ConcurrentDictionary<BindingFlagsAndReferenceHandling, EqualByFieldsSettings> Cache = new ConcurrentDictionary<BindingFlagsAndReferenceHandling, EqualByFieldsSettings>();
        private readonly HashSet<FieldInfo> ignoredFields;

        public EqualByFieldsSettings(IEnumerable<FieldInfo> ignoredFields, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            : base(bindingFlags, referenceHandling, null)
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
            return Cache.GetOrAdd(key, x => new EqualByFieldsSettings(null, bindingFlags, referenceHandling));
        }

        public bool IsIgnoringField(FieldInfo fieldInfo)
        {
            if (fieldInfo == null || fieldInfo.IsEventField())
            {
                return true;
            }

            if (this.IsIgnoringType(fieldInfo.DeclaringType))
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