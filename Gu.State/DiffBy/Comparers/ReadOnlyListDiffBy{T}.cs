namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal class ReadOnlyListDiffBy<T> : ListDiffBy, IDiffBy
    {
        public static readonly ReadOnlyListDiffBy<T> Default = new ReadOnlyListDiffBy<T>();

        private ReadOnlyListDiffBy()
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
            this.AddDiffs(collectionBuilder, (IReadOnlyList<T>)x, (IReadOnlyList<T>)y, settings, itemDiff);
        }

        private void AddDiffs<TSettings>(
            DiffBuilder collectionBuilder,
            IReadOnlyList<T> x,
            IReadOnlyList<T> y,
            TSettings settings,
            Action<DiffBuilder, object, object, object, TSettings> itemDiff)
            where TSettings : IMemberSettings
        {
            for (var i = 0; i < Math.Max(x.Count, y.Count); i++)
            {
                var xv = x.ElementAtOrMissing(i);
                var yv = y.ElementAtOrMissing(i);
                itemDiff(collectionBuilder, xv, yv, i, settings);
            }
        }
    }
}