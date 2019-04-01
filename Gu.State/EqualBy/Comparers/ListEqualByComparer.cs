namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class ListEqualByComparer
    {
        private static readonly ConcurrentDictionary<Type, EqualByComparer> Cache = new ConcurrentDictionary<Type, EqualByComparer>();

        internal static bool TryGetOrCreate(object x, object y, out EqualByComparer comparer)
        {
            if (x.GetType().Implements(typeof(IList<>)) && y.GetType().Implements(typeof(IList<>)))
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
            //// ReSharper disable once PossibleNullReferenceException nope, not here
            var comparer = (EqualByComparer)typeof(ListEqualByComparer<>).MakeGenericType(itemType)
                                                                     .GetField(nameof(ListEqualByComparer<int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                                     .GetValue(null);
            return comparer;
        }
    }
}