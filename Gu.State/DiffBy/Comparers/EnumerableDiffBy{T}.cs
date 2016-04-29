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
            object x,
            object y,
            TSettings settings,
            DiffBuilder collectionBuilder,
            Action<object, object, object, TSettings, DiffBuilder> itemDiff)
            where TSettings : IMemberSettings
        {
            this.AddDiffs((IEnumerable<T>)x, (IEnumerable<T>)y, settings, collectionBuilder, itemDiff);
        }

        private void AddDiffs<TSettings>(
            IEnumerable<T> x,
            IEnumerable<T> y,
            TSettings settings,
            DiffBuilder collectionBuilder,
            Action<object, object, object, TSettings, DiffBuilder> itemDiff)
                where TSettings : IMemberSettings
        {
            var i = -1;
            foreach (var pair in new PaddedPairs(x, y))
            {
                i++;
                itemDiff(pair.X, pair.Y, new Skip(i), settings, collectionBuilder);
            }
        }
    }
}