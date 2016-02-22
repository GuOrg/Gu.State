namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    internal static partial class TypeExt
    {
        private static readonly ConcurrentSet<Type> ImmutableTypes = new ConcurrentSet<Type>
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

        private static readonly ConcurrentSet<Type> MutableTypes = new ConcurrentSet<Type>();

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

            return IsImmutable(type, null);
        }

        private static bool IsImmutable(Type type, List<Type> checkedTypes)
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
                if (!propertyInfo.IsGetReadOnly() ||
                    (propertyInfo.GetIndexParameters().Length > 0 && propertyInfo.SetMethod != null))
                {
                    MutableTypes.Add(type);
                    return false;
                }

                if (!IsValidSubPropertyOrFieldType(propertyInfo.PropertyType))
                {
                    MutableTypes.Add(type);
                    return false;
                }

                if (!ShouldCheckPropertyOrField(type, ref checkedTypes))
                {
                    continue;
                }

                if (!IsImmutable(propertyInfo.PropertyType, checkedTypes))
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

                if (!IsValidSubPropertyOrFieldType(fieldInfo.FieldType))
                {
                    MutableTypes.Add(type);
                    return false;
                }

                if (!ShouldCheckPropertyOrField(type, ref checkedTypes))
                {
                    continue;
                }

                if (!IsImmutable(fieldInfo.FieldType, checkedTypes))
                {
                    MutableTypes.Add(type);
                    return false;
                }
            }

            ImmutableTypes.Add(type);
            return true;
        }

        private static bool IsValidSubPropertyOrFieldType(Type type)
        {
            return type.IsValueType || type.IsSealed;
        }

        private static bool ShouldCheckPropertyOrField(Type type, ref List<Type> checkedTypes)
        {
            if (checkedTypes == null)
            {
                checkedTypes = new List<Type>(1) { type };
            }
            else
            {
                if (checkedTypes.Contains(type))
                {
                    return false;
                }

                checkedTypes.Add(type);
            }

            return true;
        }
    }
}
