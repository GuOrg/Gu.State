﻿namespace Gu.State
{
    using System.Collections.Generic;

    internal class EnumerableDiffBy<T> : EnumerableDiffBy, IDiffBy
    {
        public static readonly EnumerableDiffBy<T> Default = new EnumerableDiffBy<T>();

        private EnumerableDiffBy()
        {
        }

        public void AddDiffs(
            DiffBuilder collectionBuilder,
            object x,
            object y,
            IMemberSettings settings)
        {
            this.AddDiffs(collectionBuilder, (IEnumerable<T>)x, (IEnumerable<T>)y, settings);
        }

        private void AddDiffs(
            DiffBuilder collectionBuilder,
            IEnumerable<T> x,
            IEnumerable<T> y,
            IMemberSettings settings)
        {
            var i = -1;
            foreach (var pair in new PaddedPairs(x, y))
            {
                i++;
                collectionBuilder.UpdateCollectionItemDiff(pair.X, pair.Y, new Skip(i), settings);
            }
        }
    }
}