namespace Gu.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class EqualByPropertiesSettings : EqualBySettings, IEqualByPropertiesSettings
    {
        private readonly HashSet<PropertyInfo> ignoredProperties;

        public EqualByPropertiesSettings(IEnumerable<PropertyInfo> ignoredProperties, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            : base(bindingFlags, referenceHandling)
        {
            this.ignoredProperties = ignoredProperties != null
                                         ? new HashSet<PropertyInfo>(ignoredProperties)
                                         : null;
        }

        public IEnumerable<PropertyInfo> IgnoredProperties => this.ignoredProperties ?? Enumerable.Empty<PropertyInfo>();

        public bool IsIgnoringProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null || propertyInfo.GetIndexParameters().Length > 0)
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