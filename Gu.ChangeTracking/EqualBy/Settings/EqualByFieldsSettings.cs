namespace Gu.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class EqualByFieldsSettings : EqualBySettings
    {
        private readonly HashSet<FieldInfo> ignoredFields;

        public EqualByFieldsSettings(IEnumerable<FieldInfo> ignoredFields, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            : base(bindingFlags, referenceHandling)
        {
            this.ignoredFields = ignoredFields != null
                                     ? new HashSet<FieldInfo>(ignoredFields)
                                     : null;
        }

        public IEnumerable<FieldInfo> IgnoredFields => this.ignoredFields ?? Enumerable.Empty<FieldInfo>();

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