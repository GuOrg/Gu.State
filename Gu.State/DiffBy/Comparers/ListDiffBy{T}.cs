namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal sealed class ListDiffBy<T> : ListDiffBy, IDiffBy
    {
        internal static readonly ListDiffBy<T> Default = new();

        private ListDiffBy()
        {
        }

        public void AddDiffs(
            DiffBuilder builder,
            object x,
            object y,
            MemberSettings settings)
        {
            AddDiffs(builder, (IList<T>)x, (IList<T>)y, settings);
        }

        private static void AddDiffs(
            DiffBuilder builder,
            IList<T> x,
            IList<T> y,
            MemberSettings settings)
        {
            for (var i = 0; i < Math.Max(x.Count, y.Count); i++)
            {
                var xv = x.ElementAtOrMissing(i);
                var yv = y.ElementAtOrMissing(i);
                builder.UpdateCollectionItemDiff(xv, yv, i, settings);
            }
        }
    }
}
