namespace Gu.State
{
    using System.Collections.Generic;

    internal sealed class DictionaryDiffBy<TKey, TValue> : DictionaryDiffBy, IDiffBy
    {
        internal static readonly DictionaryDiffBy<TKey, TValue> Default = new();

        private DictionaryDiffBy()
        {
        }

        public void AddDiffs(
            DiffBuilder builder,
            object x,
            object y,
            MemberSettings settings)
        {
            AddDiffs(builder, (IDictionary<TKey, TValue>)x, (IDictionary<TKey, TValue>)y, settings);
        }

        private static void AddDiffs(
            DiffBuilder builder,
            IDictionary<TKey, TValue> x,
            IDictionary<TKey, TValue> y,
            MemberSettings settings)
        {
            using var borrow = HashSetPool<TKey>.Borrow(EqualityComparer<TKey>.Default.Equals, EqualityComparer<TKey>.Default.GetHashCode);
            borrow.Value.UnionWith(x.Keys);
            borrow.Value.UnionWith(y.Keys);
            foreach (var key in borrow.Value)
            {
                var xv = x.ElementAtOrMissing(key);
                var yv = y.ElementAtOrMissing(key);
                builder.UpdateCollectionItemDiff(xv, yv, key, settings);
            }
        }
    }
}
