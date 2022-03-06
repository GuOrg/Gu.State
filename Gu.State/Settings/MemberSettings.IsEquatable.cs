namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Globalization;

    /// <summary>
    /// Base type for settings.
    /// </summary>
    public abstract partial class MemberSettings
    {
        private static readonly ConcurrentDictionary<Type, bool> EquatableCheckedTypes = new()
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

        /// <summary>
        /// Check if <paramref name="type"/> is equatable.
        /// </summary>
        /// <param name="type">The <see cref="Type"/>.</param>
        /// <returns>True if <paramref name="type"/> is equatable.</returns>
        protected static bool IsEquatableCore(Type type)
        {
            if (type is null)
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
