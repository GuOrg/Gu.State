namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class SetEqualByComparer
    {
        internal static bool TryGet(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (type.Implements(typeof(ISet<>)))
            {
                var itemType = type.GetItemType();

                // resolve comparer so we throw as early as possible if there are errors.
                _ = settings.GetEqualByComparer(itemType);
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
            public override bool Equals(
                object x,
                object y,
                MemberSettings settings,
                ReferencePairCollection referencePairs)
            {
                if (TryGetEitherNullEquals(x, y, out var result))
                {
                    return result;
                }

                var xs = (ISet<T>)x;
                var ys = (ISet<T>)y;
                if (xs.Count != ys.Count)
                {
                    return false;
                }

                var isEquatable = settings.IsEquatable(x.GetType().GetItemType());
                var xHashSet = xs as HashSet<T>;
                if (isEquatable)
                {
                    if (Equals(xHashSet?.Comparer, EqualityComparer<T>.Default))
                    {
                        return xs.SetEquals(ys);
                    }

                    return this.ItemsEquals(xs, ys, EqualityComparer<T>.Default.Equals, EqualityComparer<T>.Default.GetHashCode);
                }

                if (settings.ReferenceHandling == ReferenceHandling.References)
                {
                    return this.ItemsEquals(xs, ys, (xi, yi) => ReferenceEquals(xi, yi), xi => RuntimeHelpers.GetHashCode(xi));
                }

                var hashCodeMethod = typeof(T).GetMethod(nameof(this.GetHashCode), BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                if (hashCodeMethod.DeclaringType == typeof(object))
                {
                    return this.ItemsEquals(xs, ys, (xi, yi) => EqualBy.MemberValues(xi, yi, settings, referencePairs), _ => 0);
                }

                return this.ItemsEquals(xs, ys, (xi, yi) => EqualBy.MemberValues(xi, yi, settings, referencePairs), xi => xi.GetHashCode());
            }

            private bool ItemsEquals(ISet<T> x, ISet<T> y, Func<T, T, bool> compare, Func<T, int> getHashCode)
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