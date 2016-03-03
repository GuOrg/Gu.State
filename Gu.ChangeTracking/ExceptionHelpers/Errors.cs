namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal sealed class Errors : IErrors
    {
        private readonly Type type;

        public Errors(Type type)
        {
            this.type = type;
        }

        List<Type> IErrors.UnsupportedTypes { get; } = new List<Type>();

        List<PropertyInfo> IErrors.UnsupportedIndexers { get; } = new List<PropertyInfo>();

        List<PropertyInfo> IErrors.UnsupportedProperties { get; } = new List<PropertyInfo>();

        List<FieldInfo> IErrors.UnsupportedFields { get; } = new List<FieldInfo>();

        Type IErrors.Type => this.type;
    }
}