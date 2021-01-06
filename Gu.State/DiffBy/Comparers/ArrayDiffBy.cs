namespace Gu.State
{
    using System;

    internal sealed class ArrayDiffBy : IDiffBy
    {
        internal static readonly ArrayDiffBy Default = new ArrayDiffBy();

        public void AddDiffs(
            DiffBuilder builder,
            object x,
            object y,
            MemberSettings settings)
        {
            AddDiffs(builder, (Array)x, (Array)y, settings);
        }

        internal static bool TryGetOrCreate(object x, object y, out IDiffBy result)
        {
            if (x is Array && y is Array)
            {
                result = Default;
                return true;
            }

            result = null;
            return false;
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

        private static void AddDiffs(
            DiffBuilder builder,
            Array x,
            Array y,
            MemberSettings settings)
        {
            if (TryGetRankDiff(x, y, out var rankDiff))
            {
                builder.Add(rankDiff);
                return;
            }

            foreach (var index in x.Indices())
            {
                builder.UpdateCollectionItemDiff(x.GetValue(index), y.GetValue(index), new Index(index), settings);
            }
        }
    }
}