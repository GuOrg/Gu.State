namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal class ListDiffBy<T> : ListDiffBy, IDiffBy
    {
        public static readonly ListDiffBy<T> Default = new ListDiffBy<T>();

        private ListDiffBy()
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
            this.AddDiffs((IList<T>)x, (IList<T>)y, settings, collectionBuilder, itemDiff);
        }

        private void AddDiffs<TSettings>(
            IList<T> x,
            IList<T> y,
            TSettings settings,
            DiffBuilder collectionBuilder,
            Action<object, object, object, TSettings, DiffBuilder> itemDiff)
                where TSettings : IMemberSettings
        {
            for (int i = 0; i < Math.Max(x.Count, y.Count); i++)
            {
                var xv = x.ElementAtOrMissing(i);
                var yv = y.ElementAtOrMissing(i);
                itemDiff(xv, yv, i, settings, collectionBuilder);
            }
        }
    }
}
