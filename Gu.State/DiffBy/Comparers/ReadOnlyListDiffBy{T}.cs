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
            object x,
            object y,
            TSettings settings,
            DiffBuilder collectionBuilder,
            Action<object, object, object, TSettings, DiffBuilder> itemDiff)
            where TSettings : IMemberSettings
        {
            this.AddDiffs((IReadOnlyList<T>)x, (IReadOnlyList<T>)y, settings, collectionBuilder, itemDiff);
        }

        private void AddDiffs<TSettings>(
            IReadOnlyList<T> x,
            IReadOnlyList<T> y,
            TSettings settings,
            DiffBuilder collectionBuilder,
            Action<object, object, object, TSettings, DiffBuilder> itemDiff)
            where TSettings : IMemberSettings
        {
            for (var i = 0; i < Math.Max(x.Count, y.Count); i++)
            {
                var xv = x.ElementAtOrMissing(i);
                var yv = y.ElementAtOrMissing(i);
                itemDiff(xv, yv, i, settings, collectionBuilder);
            }
        }
    }
}