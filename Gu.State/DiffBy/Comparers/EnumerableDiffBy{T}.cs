namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal class EnumerableDiffBy<T> : EnumerableDiffBy, IDiffBy
    {
        public static readonly EnumerableDiffBy<T> Default = new EnumerableDiffBy<T>();

        private EnumerableDiffBy()
        {
        }

        public void AddDiffs<TSettings>(
            DiffBuilder collectionBuilder,
            object x,
            object y,
            TSettings settings,
            Action<DiffBuilder, object, object, object, TSettings> itemDiff)
            where TSettings : IMemberSettings
        {
            this.AddDiffs(collectionBuilder, (IEnumerable<T>)x, (IEnumerable<T>)y, settings, itemDiff);
        }

        private void AddDiffs<TSettings>(
            DiffBuilder collectionBuilder,
            IEnumerable<T> x,
            IEnumerable<T> y,
            TSettings settings,
            Action<DiffBuilder, object, object, object, TSettings> itemDiff)
                where TSettings : IMemberSettings
        {
            var i = -1;
            foreach (var pair in new PaddedPairs(x, y))
            {
                i++;
                itemDiff(collectionBuilder, pair.X, pair.Y, new Skip(i), settings);
            }
        }
    }
}