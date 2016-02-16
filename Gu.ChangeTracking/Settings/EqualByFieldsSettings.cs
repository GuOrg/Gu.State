namespace Gu.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class EqualByFieldsSettings
    {
        private readonly HashSet<FieldInfo> ignoredFields;

        public EqualByFieldsSettings(IEnumerable<FieldInfo> ignoredFields, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            this.ignoredFields = this.ignoredFields != null
                                     ? new HashSet<FieldInfo>(ignoredFields)
                                     : null;
            this.BindingFlags = bindingFlags;
            this.ReferenceHandling = referenceHandling;
        }

        public IEnumerable<FieldInfo> IgnoredFields => this.ignoredFields ?? Enumerable.Empty<FieldInfo>();

        public BindingFlags BindingFlags { get; }

        public ReferenceHandling ReferenceHandling { get; }

        public bool IsIgnoringField(FieldInfo fieldInfo)
        {
            if (this.ignoredFields == null)
            {
                return false;
            }

            return this.ignoredFields.Contains(fieldInfo);
        }
    }
}