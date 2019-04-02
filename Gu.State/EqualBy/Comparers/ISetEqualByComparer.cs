namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class ISetEqualByComparer
    {
        internal static bool TryGet(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (type.Implements(typeof(ISet<>)))
            {
                var itemType = type.GetItemType();

                // resolve comparer so we throw as early as possible if there are errors.
                _ = settings.GetEqualByComparer(itemType, checkReferenceHandling: true);
                comparer = (EqualByComparer)typeof(Comparer<>).MakeGenericType(itemType)
                                                                        .GetField(nameof(Comparer<int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                                        .GetValue(null);
                return true;
            }

            comparer = null;
            return false;
        }

        private class Comparer<T> : EqualByComparer
        {
            public static readonly Comparer<T> Default = new Comparer<T>();

            private Comparer()
            {
            }

            /// <inheritdoc />
            public override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                if (TryGetEitherNullEquals(x, y, out var result))
                {
                    return result;
                }

                return Equals((IReadOnlyCollection<T>)x, (IReadOnlyCollection<T>)y, settings, referencePairs);
            }

            private static bool Equals(IReadOnlyCollection<T> x, IReadOnlyCollection<T> y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                if (x.Count != y.Count)
                {
                    return false;
                }

                if (x is HashSet<T> xs &&
                    typeof(T).IsSealed &&
                    settings.IsEquatable(x.GetType().GetItemType()))
                {
                    return xs.SetEquals(y);
                }

                var comparer = settings.GetEqualByComparer(typeof(T), checkReferenceHandling: true);
                return ItemsEquals(x, y, (xi, yi) => comparer.Equals(xi, yi, settings, referencePairs), xi => xi.GetHashCode());
            }

            private static bool ItemsEquals(IReadOnlyCollection<T> x, IReadOnlyCollection<T> y, Func<T, T, bool> compare, Func<T, int> getHashCode)
            {
                using (var borrow = HashSetPool<T>.Borrow(compare, getHashCode))
                {
                    borrow.Value.UnionWith(x);
                    var result = borrow.Value.SetEquals(y);
                    return result;
                }
            }
        }
    }
}