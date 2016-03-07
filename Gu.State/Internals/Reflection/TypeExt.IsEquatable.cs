namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Globalization;

    internal static partial class TypeExt
    {
        private static readonly ConcurrentDictionary<Type, bool> EquatableCheckedTypes = new ConcurrentDictionary<Type, bool>
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

        public static bool IsEquatable(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            bool result;
            if (EquatableCheckedTypes.TryGetValue(type, out result))
            {
                return result;
            }

            if (type.IsNullable())
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                result = IsEquatable(underlyingType);
                EquatableCheckedTypes.TryAdd(type, result);
                return result;
            }

            if (type.IsEnum)
            {
                return true;
            }

            result = type.Implements(typeof(IEquatable<>), type);
            EquatableCheckedTypes.TryAdd(type, result);
            return result;
        }
    }
}
