namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class IReadOnlyDictionaryEqualByComparer
    {
        private static readonly ConcurrentDictionary<Type, EqualByComparer> Cache = new ConcurrentDictionary<Type, EqualByComparer>();

        internal static bool TryGet(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (type.Implements(typeof(IReadOnlyDictionary<,>)))
            {
                var iDict = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>)
                    ? type
                    : type.GetInterface("IReadOnlyDictionary`2");
                var keyType = iDict.GenericTypeArguments[0];
                // resolve comparer so we throw as early as possible if there are errors.
                _ = settings.GetEqualByComparer(keyType, checkReferenceHandling: true);
                var valueType = iDict.GenericTypeArguments[1];
                // resolve comparer so we throw as early as possible if there are errors.
                _ = settings.GetEqualByComparer(valueType, checkReferenceHandling: true);
                //// ReSharper disable once PossibleNullReferenceException nope, not here
                comparer = (EqualByComparer)typeof(Comparer<,>).MakeGenericType(keyType, valueType)
                                                               .GetField(nameof(Comparer<int, int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                               .GetValue(null);
                return true;
            }

            comparer = null;
            return false;
        }

        private class Comparer<TKey, TValue> : EqualByComparer
        {
            public static readonly Comparer<TKey, TValue> Default = new Comparer<TKey, TValue>();

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

                var xd = (IReadOnlyDictionary<TKey, TValue>)x;
                var yd = (IReadOnlyDictionary<TKey, TValue>)y;
                if (xd.Count != yd.Count)
                {
                    return false;
                }

                if (settings.IsEquatable(typeof(TValue)))
                {
                    return KeysAndValuesEquals(xd, yd, EqualityComparer<TValue>.Default.Equals);
                }

                if (settings.ReferenceHandling == ReferenceHandling.References)
                {
                    return KeysAndValuesEquals(xd, yd, (xi, yi) => ReferenceEquals(xi, yi));
                }

                return KeysAndValuesEquals(xd, yd, settings, referencePairs);
            }

            internal static bool KeysAndValuesEquals(IReadOnlyDictionary<TKey, TValue> x, IReadOnlyDictionary<TKey, TValue> y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                foreach (var key in x.Keys)
                {
                    var xv = x[key];

                    if (!y.TryGetValue(key, out var yv))
                    {
                        return false;
                    }

                    if (referencePairs?.Contains(xv, yv) == true)
                    {
                        continue;
                    }

                    if (!EqualBy.MemberValues(xv, yv, settings, referencePairs))
                    {
                        return false;
                    }
                }

                return true;
            }

            internal static bool KeysAndValuesEquals(
                IReadOnlyDictionary<TKey, TValue> x,
                IReadOnlyDictionary<TKey, TValue> y,
                Func<TValue, TValue, bool> compareItem)
            {
                foreach (var key in x.Keys)
                {
                    var xv = x[key];

                    if (!y.TryGetValue(key, out var yv))
                    {
                        return false;
                    }

                    if (!compareItem(xv, yv))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}