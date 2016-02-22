namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    internal static partial class TypeExt
    {
        private static readonly HashSet<Type> ImmutableTypes = new HashSet<Type>
                                                                   {
                                                                       typeof(Type),
                                                                       typeof(CultureInfo),
                                                                       typeof(DateTime),
                                                                       typeof(DateTime?),
                                                                       typeof(DateTimeOffset),
                                                                       typeof(DateTimeOffset?),
                                                                       typeof(TimeSpan),
                                                                       typeof(TimeSpan?),
                                                                       typeof(string),
                                                                       typeof(double),
                                                                       typeof(double?),
                                                                       typeof(float),
                                                                       typeof(float?),
                                                                       typeof(decimal),
                                                                       typeof(decimal?),
                                                                       typeof(int),
                                                                       typeof(int?),
                                                                       typeof(uint),
                                                                       typeof(uint?),
                                                                       typeof(long),
                                                                       typeof(long?),
                                                                       typeof(ulong),
                                                                       typeof(ulong?),
                                                                       typeof(short),
                                                                       typeof(short?),
                                                                       typeof(ushort),
                                                                       typeof(ushort?),
                                                                       typeof(sbyte),
                                                                       typeof(sbyte?),
                                                                       typeof(byte),
                                                                       typeof(byte?),
                                                                   };

        private static readonly HashSet<Type> MutableTypes = new HashSet<Type>();

        internal static bool IsImmutable(this Type type)
        {
            if (ImmutableTypes.Contains(type))
            {
                return true;
            }

            if (MutableTypes.Contains(type))
            {
                return false;
            }

            var propertyInfos = type.GetProperties(Constants.DefaultFieldBindingFlags);
            foreach (var propertyInfo in propertyInfos)
            {
                if (!propertyInfo.IsGetReadOnly())
                {
                    MutableTypes.Add(type);
                    return false;
                }

                if (!propertyInfo.PropertyType.IsImmutable())
                {
                    MutableTypes.Add(type);
                    return false;
                }

                if (propertyInfo.GetIndexParameters().Length > 0 && propertyInfo.SetMethod != null)
                {
                    MutableTypes.Add(type);
                    return false;
                }
            }

            var fieldInfos = type.GetFields(Constants.DefaultFieldBindingFlags);
            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.IsEventField())
                {
                    continue;
                }

                if (!fieldInfo.IsInitOnly)
                {
                    MutableTypes.Add(type);
                    return false;
                }

                if (!fieldInfo.FieldType.IsImmutable())
                {
                    MutableTypes.Add(type);
                    return false;
                }
            }

            ImmutableTypes.Add(type);
            return true;
        }
    }
}
