namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;

    public abstract partial class MemberSettings
    {
        private static readonly ConcurrentDictionary<Type, bool> ImmutableCheckedTypes = new ConcurrentDictionary<Type, bool>
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

        protected static bool IsImmutableCore(Type type)
        {
            return ImmutableCheckedTypes.GetOrAdd(type, x => CheckIfIsImmutable(x, null));
        }

        private static bool CheckIfIsImmutable(Type type, List<Type> checkedTypes)
        {
            bool result;
            if (ImmutableCheckedTypes.TryGetValue(type, out result))
            {
                return result;
            }

            if (type.IsNullable())
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                result = CheckIfIsImmutable(underlyingType, checkedTypes);
            }
            else if (type.IsEnum)
            {
                result = true;
            }
            else if (type.IsDelegate())
            {
                result = true;
            }
            else if (type.IsImmutableList() ||
                     type.IsImmutableArray() ||
                     type.IsImmutableHashSet())
            {
                var itemType = type.GetItemType();
                result = CheckIfIsImmutable(itemType, checkedTypes);
            }
            else if (type.IsKeyValuePair())
            {
                var genericArguments = type.GetGenericArguments();
                result = CheckIfIsImmutable(genericArguments[0], checkedTypes)
                         && CheckIfIsImmutable(genericArguments[1], checkedTypes);
            }
            else if (CanBeImmutable(type))
            {
                result = HasImmutableMembers(type, checkedTypes);
            }
            else
            {
                result = false;
            }

            ImmutableCheckedTypes.TryAdd(type, result);
            return result;
        }

        private static bool HasImmutableMembers(Type type, List<Type> checkedTypes)
        {
            var propertyInfos = type.GetProperties(Constants.DefaultFieldBindingFlags);
            foreach (var propertyInfo in propertyInfos)
            {
                if (!propertyInfo.IsGetReadOnly() || (propertyInfo.GetIndexParameters()
                                                                  .Length > 0 && propertyInfo.SetMethod != null))
                {
                    return false;
                }

                if (!CanBeImmutable(propertyInfo.PropertyType))
                {
                    return false;
                }

                if (!ShouldCheckPropertyOrField(type, ref checkedTypes))
                {
                    continue;
                }

                if (!CheckIfIsImmutable(propertyInfo.PropertyType, checkedTypes))
                {
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
                    return false;
                }

                if (!CanBeImmutable(fieldInfo.FieldType))
                {
                    return false;
                }

                if (!ShouldCheckPropertyOrField(type, ref checkedTypes))
                {
                    continue;
                }

                if (!CheckIfIsImmutable(fieldInfo.FieldType, checkedTypes))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CanBeImmutable(Type type)
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
