namespace Gu.ChangeTracking
{
    using System.Collections.Generic;
    using System.Reflection;

    public interface IEqualByPropertiesSettings : IEqualBySettings
    {
        IEnumerable<PropertyInfo> IgnoredProperties { get; }

        BindingFlags BindingFlags { get; }

        ReferenceHandling ReferenceHandling { get; }

        bool IsIgnoringProperty(PropertyInfo propertyInfo);
    }
}