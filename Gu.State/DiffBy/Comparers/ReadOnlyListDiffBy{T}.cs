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

        public void AddDiffs(
            DiffBuilder collectionBuilder,
            object x,
            object y,
            MemberSettings settings)
        {
            this.AddDiffs(collectionBuilder, (IReadOnlyList<T>)x, (IReadOnlyList<T>)y, settings);
        }

        private void AddDiffs(
            DiffBuilder collectionBuilder,
            IReadOnlyList<T> x,
            IReadOnlyList<T> y,
            MemberSettings settings)
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