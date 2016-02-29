namespace Gu.ChangeTracking
{
    using System.Collections.Generic;
    using System.Reflection;

    public interface IEqualByFieldsSettings : IEqualBySettings
    {
        IEnumerable<FieldInfo> IgnoredFields { get; }

        BindingFlags BindingFlags { get; }

        ReferenceHandling ReferenceHandling { get; }

        bool IsIgnoringField(FieldInfo propertyInfo);
    }
}