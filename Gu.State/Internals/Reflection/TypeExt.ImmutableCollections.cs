namespace Gu.State
{
    using System;

    internal static partial class TypeExt
    {
        internal static bool IsImmutableList(this Type type)
        {
            return type.FullName.StartsWith("System.Collections.Immutable.ImmutableList`1");
        }

        internal static bool IsImmutableArray(this Type type)
        {
            return type.FullName.StartsWith("System.Collections.Immutable.ImmutableArray`1");
        }

        internal static bool IsImmutableHashSet(this Type type)
        {
            return type.FullName.StartsWith("System.Collections.Immutable.ImmutableHashSet`1");
        }

        internal static bool IsImmutableDictionary(this Type type)
        {
            return type.FullName.StartsWith("System.Collections.Immutable.ImmutableDictionary`2");
        }
    }
}