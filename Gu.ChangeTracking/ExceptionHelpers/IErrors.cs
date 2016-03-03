namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal interface IErrors
    {
        List<Type> UnsupportedTypes { get; }

        List<PropertyInfo> UnsupportedIndexers { get; }

        List<PropertyInfo> UnsupportedProperties { get; }

        List<FieldInfo> UnsupportedFields { get; }

        Type Type { get; }
    }
}