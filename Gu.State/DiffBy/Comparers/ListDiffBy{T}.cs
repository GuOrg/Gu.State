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

        public void AddDiffs(
            DiffBuilder collectionBuilder,
            object x,
            object y,
            IMemberSettings settings)
        {
            this.AddDiffs(collectionBuilder, (IList<T>)x, (IList<T>)y, settings);
        }

        private void AddDiffs<TSettings>(
            DiffBuilder collectionBuilder,
            IList<T> x,
            IList<T> y,
            TSettings settings)
                where TSettings : IMemberSettings
        {
            for (var i = 0; i < Math.Max(x.Count, y.Count); i++)
            {
                var xv = x.ElementAtOrMissing(i);
                var yv = y.ElementAtOrMissing(i);
                collectionBuilder.UpdateCollectionItemDiff(xv, yv, i, settings);
            }
        }
    }
}
