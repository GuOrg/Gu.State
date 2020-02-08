namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    internal class ReadOnlyDictionaryDiffBy
    {
        private static readonly ConcurrentDictionary<Type, IDiffBy> Cache = new ConcurrentDictionary<Type, IDiffBy>();

        internal static bool TryGetOrCreate(object x, object y, out IDiffBy comparer)
        {
            if (x.GetType().Implements(typeof(IReadOnlyDictionary<,>)) && y.GetType().Implements(typeof(IReadOnlyDictionary<,>)))
            {
                comparer = Cache.GetOrAdd(x.GetType(), Create);
                return true;
            }

            comparer = null;
            return false;
        }

        private static IDiffBy Create(Type type)
        {
            var iDict = type.Name == "IReadOnlyDictionary`2" ? type : type.GetInterface("IReadOnlyDictionary`2");
            var keyType = iDict.GenericTypeArguments[0];
            var valueType = iDict.GenericTypeArguments[1];
            //// ReSharper disable once PossibleNullReferenceException nope, not here
            var comparer = (IDiffBy)typeof(ReadOnlyDictionaryDiffBy<,>).MakeGenericType(keyType, valueType)
                                                                       .GetField(nameof(ReadOnlyDictionaryDiffBy<int, int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                                       .GetValue(null);
            return comparer;
        }
    }
}
