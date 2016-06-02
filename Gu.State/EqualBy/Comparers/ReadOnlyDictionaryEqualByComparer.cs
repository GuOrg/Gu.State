namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class ReadOnlyDictionaryEqualByComparer
    {
        private static readonly ConcurrentDictionary<Type, EqualByComparer> Cache = new ConcurrentDictionary<Type, EqualByComparer>();

        private static readonly string IReadOnlyDictionaryName = "IReadOnlyDictionary`2";

        public static bool TryGetOrCreate(object x, object y, out EqualByComparer comparer)
        {
            if (x.GetType().Implements(typeof(IReadOnlyDictionary<,>)) && y.GetType().Implements(typeof(IReadOnlyDictionary<,>)))
            {
                comparer = Cache.GetOrAdd(x.GetType(), Create);
                return true;
            }

            comparer = null;
            return false;
        }

        private static EqualByComparer Create(Type type)
        {
            var iDict = type.Name == IReadOnlyDictionaryName ? type : type.GetInterface(IReadOnlyDictionaryName);
            var keyType = iDict.GenericTypeArguments[0];
            var valueType = iDict.GenericTypeArguments[1];
            //// ReSharper disable once PossibleNullReferenceException nope, not here
            var comparer = (EqualByComparer)typeof(ReadOnlyDictionaryEqualByComparer<,>).MakeGenericType(keyType, valueType)
                                                                                        .GetField(nameof(ReadOnlyDictionaryEqualByComparer<int, int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                                                        .GetValue(null);
            return comparer;
        }
    }
}