namespace Gu.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class EqualByPropertiesSettings
    {
        private readonly HashSet<PropertyInfo> ignoredProperties;

        public EqualByPropertiesSettings(IEnumerable<PropertyInfo> ignoredProperties, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            this.ignoredProperties = this.ignoredProperties != null
                                         ? new HashSet<PropertyInfo>(ignoredProperties)
                                         : null;
            this.BindingFlags = bindingFlags;
            this.ReferenceHandling = referenceHandling;
        }

        public IEnumerable<PropertyInfo> IgnoredProperties => this.ignoredProperties ?? Enumerable.Empty<PropertyInfo>();

        public BindingFlags BindingFlags { get; }

        public ReferenceHandling ReferenceHandling { get; }

        public bool IsIgnoringProperty(PropertyInfo propertyInfo)
        {
            if (this.ignoredProperties == null)
            {
                return false;
            }

            return this.ignoredProperties.Contains(propertyInfo);
        }
    }
}