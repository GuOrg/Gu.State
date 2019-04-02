namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class EquatableEqualByComparer
    {
        internal static bool TryGet(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (settings.IsEquatable(type) &&
                type.Name != "ImmutableArray`1")
            {
                if (type.IsNullable())
                {
                    comparer = (EqualByComparer)typeof(NullableComparer<>).MakeGenericType(type.GenericTypeArguments)
                                                                  .GetField(nameof(NullableComparer<int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                                  .GetValue(null);
                }
                else
                {
                    comparer = (EqualByComparer)typeof(Comparer<>).MakeGenericType(type)
                                                                  .GetField(nameof(Comparer<int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                                  .GetValue(null);
                }

                return true;
            }

            comparer = null;
            return false;
        }

        private class Comparer<T> : EqualByComparer
        {
            public static Comparer<T> Default = new Comparer<T>(EqualityComparer<T>.Default);
            private readonly EqualityComparer<T> comparer;

            private Comparer(EqualityComparer<T> comparer)
            {
                this.comparer = comparer;
            }

            public override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                return TryGetEitherNullEquals(x, y, out var result)
                    ? result
                    : this.comparer.Equals((T)x, (T)y);
            }
        }

        private class NullableComparer<T> : EqualByComparer
            where T : struct
        {
            public static NullableComparer<T> Default = new NullableComparer<T>();

            private NullableComparer()
            {
            }

            public override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                return Nullable.Equals((T?)x, (T?)y);
            }
        }
    }
}
