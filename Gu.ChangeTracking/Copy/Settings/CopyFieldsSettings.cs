namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class CopyFieldsSettings : CopySettings, IEqualByFieldsSettings
    {
        private static readonly ConcurrentDictionary<BindingFlagsAndReferenceHandling, CopyFieldsSettings> Cache = new ConcurrentDictionary<BindingFlagsAndReferenceHandling, CopyFieldsSettings>();

        private readonly HashSet<FieldInfo> ignoredFields;

        public CopyFieldsSettings(IEnumerable<FieldInfo> ignoredFields, IEnumerable<Type> ignoredTypes, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            : base(bindingFlags, referenceHandling, ignoredTypes)
        {
            this.ignoredFields = ignoredFields != null
                                     ? new HashSet<FieldInfo>(ignoredFields)
                                     : null;
        }

        public IEnumerable<FieldInfo> IgnoredFields => this.ignoredFields ?? Enumerable.Empty<FieldInfo>();

        public static CopyFieldsSettings Create<T>(T source, T target, BindingFlags bindingFlags, ReferenceHandling referenceHandling, string[] excludedFields)
        {
            var type = source?.GetType() ?? target?.GetType() ?? typeof(T);
            return Create(type, bindingFlags, referenceHandling, excludedFields);
        }

        public static CopyFieldsSettings Create<T>(BindingFlags bindingFlags, ReferenceHandling referenceHandling, string[] excludedFields)
        {
            return Create(typeof(T), bindingFlags, referenceHandling, excludedFields);
        }

        public static CopyFieldsSettings Create(
            Type type,
            BindingFlags bindingFlags,
            ReferenceHandling referenceHandling,
            string[] excludedFields)
        {
            var ignoreFields = type.GetIgnoreFields(bindingFlags, excludedFields);
            if (ignoreFields == null || ignoreFields.Count == 0)
            {
                return GetOrCreate(bindingFlags, referenceHandling);
            }

            return new CopyFieldsSettings(ignoreFields, null, bindingFlags, referenceHandling);
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
            return Cache.GetOrAdd(key, x => new CopyFieldsSettings(null, null, bindingFlags, referenceHandling));
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

            return this.ignoredFields?.Contains(fieldInfo) == true;
        }
    }
}