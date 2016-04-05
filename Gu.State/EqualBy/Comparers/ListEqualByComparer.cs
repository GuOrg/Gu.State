namespace Gu.State
{
    using System.Collections.Generic;
    using System.Reflection;

    internal static class ListEqualByComparer
    {
        public static bool TryGetOrCreate(object x, object y, out EqualByComparer comparer)
        {
            if (x.GetType().Implements(typeof(IList<>)) && y.GetType().Implements(typeof(IList<>)))
            {
                var itemType = x.GetType().GetItemType();
                comparer = (EqualByComparer)typeof(ListEqualByComparer<>).MakeGenericType(itemType)
                                                                         .GetField(nameof(ListEqualByComparer<int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                                         .GetValue(null);
                return true;
            }

            comparer = null;
            return false;
        }
    }
}