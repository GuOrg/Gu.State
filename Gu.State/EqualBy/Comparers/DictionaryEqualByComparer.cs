namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class DictionaryEqualByComparer
    {
        private static readonly ConcurrentDictionary<Type, EqualByComparer> Cache = new ConcurrentDictionary<Type, EqualByComparer>();

        private static readonly string IDictionaryName = "IDictionary`2";

        public static bool TryGetOrCreate(object x, object y, out EqualByComparer comparer)
        {
            if (x.GetType().Implements(typeof(IDictionary<,>)) && y.GetType().Implements(typeof(IDictionary<,>)))
            {
                comparer = Cache.GetOrAdd(x.GetType(), Create);
                return true;
            }

            comparer = null;
            return false;
        }

        private static EqualByComparer Create(Type type)
        {
            var iDict = type.Name == IDictionaryName ? type : type.GetInterface(IDictionaryName);
            var keyType = iDict.GenericTypeArguments[0];
            var valueType = iDict.GenericTypeArguments[1];
            var comparer = (EqualByComparer)typeof(DictionaryEqualByComparer<,>).MakeGenericType(keyType, valueType)
                                                                                .GetField(nameof(DictionaryEqualByComparer<int, int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                                                .GetValue(null);
            return comparer;
        }
    }
}