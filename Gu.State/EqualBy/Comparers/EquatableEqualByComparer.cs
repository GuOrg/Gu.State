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
                comparer = (EqualByComparer)typeof(Comparer<>).MakeGenericType(type)
                                                              .GetField(nameof(Comparer<int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                              .GetValue(null);
                return true;
            }

            comparer = null;
            return false;
        }

        private class Comparer<T> : EqualByComparer
            where T : IEquatable<T>
        {
            public static Comparer<T> Default = new Comparer<T>(EqualityComparer<T>.Default);
            private readonly EqualityComparer<T> comparer;

            private Comparer(EqualityComparer<T> comparer)
            {
                this.comparer = comparer;
            }

            public override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                return this.comparer.Equals((T)x, (T)y);
            }
        }
    }
}
