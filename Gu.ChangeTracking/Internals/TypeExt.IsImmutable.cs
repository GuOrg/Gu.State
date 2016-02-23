namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;

    internal static partial class TypeExt
    {
        private static readonly ConcurrentDictionary<Type, bool> CheckedTypes = new ConcurrentDictionary<Type, bool>
        {
            [typeof(Type)] = true,
            [typeof(CultureInfo)] = true,
            [typeof(DateTime)] = true,
            [typeof(DateTimeOffset)] = true,
            [typeof(TimeSpan)] = true,
            [typeof(string)] = true,
            [typeof(double)] = true,
            [typeof(float)] = true,
            [typeof(decimal)] = true,
            [typeof(int)] = true,
            [typeof(uint)] = true,
            [typeof(long)] = true,
            [typeof(ulong)] = true,
            [typeof(short)] = true,
            [typeof(ushort)] = true,
            [typeof(sbyte)] = true,
            [typeof(byte)] = true,
        };

        internal static bool IsImmutable(this Type type)
        {
            bool result;
            if (CheckedTypes.TryGetValue(type, out result))
            {
                return result;
            }

            return IsImmutable(type, null);
        }

        private static bool IsImmutable(Type type, List<Type> checkedTypes)
        {
            bool result;
            if (CheckedTypes.TryGetValue(type, out result))
            {
                return result;
            }
            if (type.IsNullable())
            {
                type = Nullable.GetUnderlyingType(type);
                var isImmutable = IsImmutable(type, checkedTypes);
                CheckedTypes.TryAdd(type, isImmutable);
                return isImmutable;
            }

            if (type.IsEnum)
            {
                CheckedTypes.TryAdd(type, true);
                return true;
            }

            var propertyInfos = type.GetProperties(Constants.DefaultFieldBindingFlags);
            foreach (var propertyInfo in propertyInfos)
            {
                if (!propertyInfo.IsGetReadOnly() ||
                    (propertyInfo.GetIndexParameters().Length > 0 && propertyInfo.SetMethod != null))
                {

                    CheckedTypes.TryAdd(type, false);
                    return false;
                }

                if (!IsValidSubPropertyOrFieldType(propertyInfo.PropertyType))
                {
                    CheckedTypes.TryAdd(type, false);
                    return false;
                }

                if (!ShouldCheckPropertyOrField(type, ref checkedTypes))
                {
                    continue;
                }

                if (!IsImmutable(propertyInfo.PropertyType, checkedTypes))
                {
                    CheckedTypes.TryAdd(type, false);
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
                    CheckedTypes.TryAdd(type, false);
                    return false;
                }

                if (!IsValidSubPropertyOrFieldType(fieldInfo.FieldType))
                {
                    CheckedTypes.TryAdd(type, false);
                    return false;
                }

                if (!ShouldCheckPropertyOrField(type, ref checkedTypes))
                {
                    continue;
                }

                if (!IsImmutable(fieldInfo.FieldType, checkedTypes))
                {
                    CheckedTypes.TryAdd(type, false);
                    return false;
                }
            }

            CheckedTypes.TryAdd(type, true);
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
