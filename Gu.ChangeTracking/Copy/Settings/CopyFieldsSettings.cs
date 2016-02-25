namespace Gu.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class CopyFieldsSettings : CopySettings
    {
        private static readonly Dictionary<BindingFlagsAndReferenceHandling, CopyFieldsSettings> Cache = new Dictionary<BindingFlagsAndReferenceHandling, CopyFieldsSettings>();

        private readonly HashSet<FieldInfo> ignoredFields;

        public CopyFieldsSettings(IEnumerable<FieldInfo> ignoredFields, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            : base(bindingFlags, referenceHandling)
        {
            this.ignoredFields = ignoredFields != null
                                     ? new HashSet<FieldInfo>(ignoredFields)
                                     : null;
        }

        public IEnumerable<FieldInfo> IgnoredFields => this.ignoredFields ?? Enumerable.Empty<FieldInfo>();

        public static CopyFieldsSettings Create<T>(T source, T target, BindingFlags bindingFlags, ReferenceHandling referenceHandling, string[] excludedFields)
        {
            var type = source?.GetType() ?? target?.GetType() ?? typeof(T);
            var ignoreFields = type.GetIgnoreFields(bindingFlags, excludedFields);
            if (ignoreFields == null || ignoreFields.Count == 0)
            {
                return GetOrCreate(bindingFlags, referenceHandling);
            }

            return new CopyFieldsSettings(ignoreFields, bindingFlags, referenceHandling);
        }

        public static CopyFieldsSettings GetOrCreate(BindingFlags bindingFlags)
        {
            return GetOrCreate(bindingFlags, ReferenceHandling.Throw);
        }

        public static CopyFieldsSettings GetOrCreate(ReferenceHandling referenceHandling)
        {
            return GetOrCreate(Constants.DefaultFieldBindingFlags, referenceHandling);
        }

        public static CopyFieldsSettings GetOrCreate(BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            var key = new BindingFlagsAndReferenceHandling(bindingFlags, referenceHandling);
            CopyFieldsSettings settings;
            if (Cache.TryGetValue(key, out settings))
            {
                return settings;
            }

            settings = new CopyFieldsSettings(null, bindingFlags, referenceHandling);
            Cache[key] = settings;
            return settings;
        }

        public bool IsIgnoringField(FieldInfo fieldInfo)
        {
            if (fieldInfo.IsEventField())
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