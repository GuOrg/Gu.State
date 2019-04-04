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
                comparer = (EqualByComparer)typeof(Comparer<,,>).MakeGenericType(type, iDict.GenericTypeArguments[0], iDict.GenericTypeArguments[1])
                                                               .GetField(nameof(Comparer<int, int, int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                               .GetValue(null);
                return true;
            }

            comparer = null;
            return false;
        }

        private class Comparer<TMap, TKey, TValue> : EqualByComparer<IReadOnlyDictionary<TKey, TValue>>
        {
            public static readonly Comparer<TMap, TKey, TValue> Default = new Comparer<TMap, TKey, TValue>();

            private Comparer()
            {
            }

            internal override bool TryGetError(MemberSettings settings, out Error error)
            {
                if (CollectionEqualByComparer<TMap, TKey>.TryGetItemError(settings, out var keyError))
                {
                    error = keyError;
                    return true;
                }

                if (CollectionEqualByComparer<TMap, TValue>.TryGetItemError(settings, out var valueError))
                {
                    error = new TypeErrors(typeof(TMap), valueError);
                    return true;
                }

                error = null;
                return false;
            }

            internal override bool Equals(IReadOnlyDictionary<TKey, TValue> x, IReadOnlyDictionary<TKey, TValue> y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                if (x.Count != y.Count)
                {
                    return false;
                }

                return ISetEqualByComparer.SetEquals(x.Keys, y.Keys, settings, referencePairs) &&
                       ValuesEquals(x, y, settings, referencePairs);
            }

            private static bool ValuesEquals(IReadOnlyDictionary<TKey, TValue> x, IReadOnlyDictionary<TKey, TValue> y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                var comparer = settings.GetEqualByComparer(typeof(TValue));
                foreach (var key in x.Keys)
                {
                    if (!y.TryGetValue(key, out var yv))
                    {
                        return false;
                    }

                    if (!comparer.Equals(x[key], yv, settings, referencePairs))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}