namespace Gu.State
{
    using System.Collections.Generic;

    internal sealed class EnumerableDiffBy<T> : EnumerableDiffBy, IDiffBy
    {
        internal static readonly EnumerableDiffBy<T> Default = new();

        private EnumerableDiffBy()
        {
        }

        public void AddDiffs(
            DiffBuilder builder,
            object x,
            object y,
            MemberSettings settings)
        {
            AddDiffs(builder, (IEnumerable<T>)x, (IEnumerable<T>)y, settings);
        }

        private static void AddDiffs(
            DiffBuilder builder,
            IEnumerable<T> x,
            IEnumerable<T> y,
            MemberSettings settings)
        {
            var i = -1;
            foreach (var pair in new PaddedPairs(x, y))
            {
                i++;
                builder.UpdateCollectionItemDiff(pair.X, pair.Y, new Skip(i), settings);
            }
        }
    }
}
