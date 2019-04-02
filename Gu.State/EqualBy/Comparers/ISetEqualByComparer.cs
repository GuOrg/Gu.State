namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class ISetEqualByComparer
    {
        internal static bool TryGet(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (type.Implements(typeof(ISet<>)))
            {
                comparer = (EqualByComparer)typeof(Comparer<>).MakeGenericType(type.GetItemType())
                                                              .GetField(nameof(Comparer<int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                              .GetValue(null);
                return true;
            }

            comparer = null;
            return false;
        }

        internal static bool SetEquals<T>(IEnumerable<T> xs, IEnumerable<T> ys, MemberSettings settings, ReferencePairCollection referencePairs)
        {
            var comparer = settings.GetEqualByComparer(typeof(T));
            if (comparer is ReferenceEqualByComparer)
            {
                using (var borrowed = HashSetPool<T>.Borrow(
                    (x, y) => comparer.Equals(x, y, settings, referencePairs),
                    x => RuntimeHelpers.GetHashCode(x)))
                {
                    borrowed.Value.UnionWith(xs);
                    return borrowed.Value.SetEquals(ys);
                }
            }

            if (typeof(T).IsSealed &&
                settings.IsEquatable(typeof(T)))
            {
                using (var borrowed = HashSetPool<T>.Borrow(
                    (x, y) => comparer.Equals(x, y, settings, referencePairs),
                    x => x.GetHashCode()))
                {
                    borrowed.Value.UnionWith(xs);
                    return borrowed.Value.SetEquals(ys);
                }
            }

            using (var borrowed = HashSetPool<T>.Borrow(
                (x, y) => comparer.Equals(x, y, settings, referencePairs),
                x => 0))
            {
                borrowed.Value.UnionWith(xs);
                return borrowed.Value.SetEquals(ys);
            }
        }

        private class Comparer<T> : EqualByComparer<IReadOnlyCollection<T>>
        {
            public static readonly Comparer<T> Default = new Comparer<T>();

            private Comparer()
            {
            }

            public override bool Equals(IReadOnlyCollection<T> x, IReadOnlyCollection<T> y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                if (x.Count != y.Count)
                {
                    return false;
                }

                if (x is HashSet<T> xs &&
                    typeof(T).IsSealed &&
                    ReferenceEquals(xs.Comparer, EqualityComparer<T>.Default) &&
                    settings.IsEquatable(x.GetType().GetItemType()))
                {
                    return xs.SetEquals(y);
                }

                return SetEquals(x, y, settings, referencePairs);
            }
        }
    }
}