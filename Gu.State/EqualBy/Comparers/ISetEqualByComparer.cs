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
            if (type.Implements(typeof(IEnumerable<>)) &&
                type.Namespace.StartsWith("System.", StringComparison.Ordinal) &&
                type.Name.EndsWith("Set`1", StringComparison.Ordinal))
            {
                comparer = (EqualByComparer)typeof(EqualByComparer<,>).MakeGenericType(type, type.GetItemType())
                                                              .GetField(nameof(EqualByComparer<IEnumerable<int>, int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
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

        private class EqualByComparer<TSet, TItem> : CollectionEqualByComparer<TSet, TItem>
            where TSet : IEnumerable<TItem>
        {
            public static readonly EqualByComparer<TSet, TItem> Default = new EqualByComparer<TSet, TItem>();

            private EqualByComparer()
            {
            }

            internal override bool Equals(TSet x, TSet y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                if (x is HashSet<TItem> hashSet &&
                    typeof(TItem).IsSealed &&
                    ReferenceEquals(hashSet.Comparer, EqualityComparer<TItem>.Default) &&
                    settings.IsEquatable(x.GetType().GetItemType()))
                {
                    return hashSet.SetEquals(y);
                }

                return SetEquals(x, y, settings, referencePairs);
            }
        }
    }
}