namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    internal static class EquatableEqualByComparer
    {
        internal static bool TryGet(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (settings.IsEquatable(type) &&
                type.Name != "ImmutableArray`1")
            {
                comparer = (EqualByComparer)typeof(Comparer<>).MakeGenericType(type)
                                                              .GetField(nameof(Comparer<int>.Default), BindingFlags.Public | BindingFlags.Static)
                                                              .GetValue(null);
                return true;
            }

            comparer = null;
            return false;
        }

        [DebuggerDisplay("EquatableEqualByComparer<{typeof(T).PrettyName()}>")]
        private class Comparer<T> : EqualByComparer<T>
        {
            public static Comparer<T> Default = new Comparer<T>(EqualityComparer<T>.Default);
            private readonly EqualityComparer<T> comparer;

            private Comparer(EqualityComparer<T> comparer)
            {
                this.comparer = comparer;
            }

            internal override bool Equals(T x, T y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
            {
                return this.comparer.Equals(x, y);
            }
        }
    }
}
