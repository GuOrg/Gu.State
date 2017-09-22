namespace Gu.State
{
    using System;

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

        public void AddDiffs(
            DiffBuilder collectionBuilder,
            object x,
            object y,
            MemberSettings settings)
        {
            this.AddDiffs(collectionBuilder, (Array)x, (Array)y, settings);
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

        private void AddDiffs(
            DiffBuilder collectionBuilder,
            Array x,
            Array y,
            MemberSettings settings)
        {
            if (TryGetRankDiff(x, y, out var rankDiff))
            {
                collectionBuilder.Add(rankDiff);
                return;
            }

            foreach (var index in x.Indices())
            {
                collectionBuilder.UpdateCollectionItemDiff(x.GetValue(index), y.GetValue(index), new Index(index), settings);
            }
        }
    }
}