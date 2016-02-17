namespace Gu.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class CopyPropertiesSettings : CopySettings
    {
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

        public IEnumerable<PropertyInfo> IgnoredProperties => this.ignoredProperties ?? Enumerable.Empty<PropertyInfo>();

        public IReadOnlyList<SpecialCopyProperty> SpecialCopyProperties { get; }

        public bool IsIgnoringProperty(PropertyInfo propertyInfo)
        {
            return this.ignoredProperties?.Contains(propertyInfo) == true;
        }

        public SpecialCopyProperty GetSpecialCopyProperty(PropertyInfo propertyInfo)
        {
            return this.SpecialCopyProperties?.SingleOrDefault(x => x.Property == propertyInfo);
        }
    }
}