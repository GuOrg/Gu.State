namespace Gu.State
{
    using System;
    using System.Reflection;

    internal class ArrayDiffBy : IDiffBy
    {
        public static readonly ArrayDiffBy Default = new ArrayDiffBy();

        public static bool TryGetOrCreate(object x, object y, out IDiffBy result)
        {
            if (x is Array && y is Array)
            {
                result = Default;
                return true;
            }

            result = null;
            return false;
        }

        public void AddDiffs<TSettings>(
            DiffBuilder collectionBuilder,
            object x,
            object y,
            TSettings settings,
            Action<DiffBuilder, object, object, object, TSettings> itemDiff)
                where TSettings : IMemberSettings
        {
            this.AddDiffs(collectionBuilder, (Array)x, (Array)y, settings, itemDiff);
        }

        private static IDiffBy Create(Type type)
        {
            var itemType = type.GetItemType();
            var comparer = (IDiffBy)typeof(ArrayDiffBy).MakeGenericType(itemType)
                                                         .GetField(nameof(Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                         .GetValue(null);
            return comparer;
        }

        private static bool TryGetRankDiff(Array x, Array y, out RankDiff rankDiff)
        {
            if (x.Length != y.Length || x.Rank != y.Rank)
            {
                rankDiff = new RankDiff(x, y);
                return true;
            }

            for (var i = 0; i < x.Rank; i++)
            {
                if (x.GetLowerBound(i) != y.GetLowerBound(i) ||
                    x.GetUpperBound(i) != y.GetUpperBound(i))
                {
                    rankDiff = new RankDiff(x, y);
                    return true;
                }
            }

            rankDiff = null;
            return false;
        }

        private void AddDiffs<TSettings>(
            DiffBuilder collectionBuilder,
            Array x,
            Array y,
            TSettings settings,
            Action<DiffBuilder, object, object, object, TSettings> itemDiff)
                where TSettings : IMemberSettings
        {
            RankDiff rankDiff;
            if (TryGetRankDiff(x, y, out rankDiff))
            {
                collectionBuilder.Add(rankDiff);
                return;
            }

            foreach (var index in x.Indices())
            {
                itemDiff(collectionBuilder, x.GetValue(index), y.GetValue(index), new Index(index), settings);
            }
        }
    }
}