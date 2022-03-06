namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    internal class SetDiffBy
    {
        private static readonly ConcurrentDictionary<Type, IDiffBy> Cache = new();

        internal static bool TryGetOrCreate(object x, object y, out IDiffBy result)
        {
            if (x.GetType().Implements(typeof(ISet<>)) && y.GetType().Implements(typeof(ISet<>)))
            {
                result = Cache.GetOrAdd(x.GetType(), Create);
                return true;
            }

            result = null;
            return false;
        }

        private static IDiffBy Create(Type type)
        {
            var itemType = type.GetItemType();
            //// ReSharper disable once PossibleNullReferenceException nope, not here
            var comparer = (IDiffBy)typeof(SetDiffBy<>).MakeGenericType(itemType)
                                                       .GetField(nameof(SetDiffBy<int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                       .GetValue(null);
            return comparer;
        }
    }
}
