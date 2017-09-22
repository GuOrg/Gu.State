namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Globalization;

    public abstract partial class MemberSettings
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

        protected static bool IsEquatableCore(Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (EquatableCheckedTypes.TryGetValue(type, out var result))
            {
                return result;
            }

            if (type.IsNullable())
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                result = IsEquatableCore(underlyingType);
                EquatableCheckedTypes.TryAdd(type, result);
                return result;
            }

            if (type.IsEnum)
            {
                result = true;
            }
            else if (type.IsInSystemCollectionsImmutable())
            {
                // special casing ImmutableArray due to weird equality
                // Implements IEquatable<ImmutableArray> but Equals does not compare elements.
                result = false;
            }
            else
            {
                result = type.Implements(typeof(IEquatable<>), type);
            }

            EquatableCheckedTypes.TryAdd(type, result);
            return result;
        }
    }
}
