namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    internal class DictionaryDiffBy
    {
        private static readonly ConcurrentDictionary<Type, IDiffBy> Cache = new();

        internal static bool TryGetOrCreate(object x, object y, out IDiffBy comparer)
        {
            if (x.GetType().Implements(typeof(IDictionary<,>)) && y.GetType().Implements(typeof(IDictionary<,>)))
            {
                comparer = Cache.GetOrAdd(x.GetType(), Create);
                return true;
            }

            comparer = null;
            return false;
        }

        private static IDiffBy Create(Type type)
        {
            var iDict = type.Name == "IDictionary`2" ? type : type.GetInterface("IDictionary`2");
            var keyType = iDict.GenericTypeArguments[0];
            var valueType = iDict.GenericTypeArguments[1];
            //// ReSharper disable once PossibleNullReferenceException nope, not here
            var comparer = (IDiffBy)typeof(DictionaryDiffBy<,>).MakeGenericType(keyType, valueType)
                                                               .GetField(nameof(DictionaryDiffBy<int, int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                               .GetValue(null);
            return comparer;
        }
    }
}
