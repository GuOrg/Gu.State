namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class EnumerableEqualByComparer
    {
        private static readonly ConcurrentDictionary<Type, EqualByComparer> Cache = new ConcurrentDictionary<Type, EqualByComparer>();

        public static bool TryGetOrCreate(object x, object y, out EqualByComparer comparer)
        {
            if (x.GetType().Implements(typeof(IEnumerable<>)) && y.GetType().Implements(typeof(IEnumerable<>)))
            {
                comparer = Cache.GetOrAdd(x.GetType(), Create);
                return true;
            }

            comparer = null;
            return false;
        }

        private static EqualByComparer Create(Type type)
        {
            var itemType = type.GetItemType();
            var comparer = (EqualByComparer)typeof(EnumerableEqualByComparer<>).MakeGenericType(itemType)
                                                                               .GetField(nameof(EnumerableEqualByComparer<int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                                               .GetValue(null);
            return comparer;
        }
    }
}