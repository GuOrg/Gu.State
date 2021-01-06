namespace Gu.State
{
    using System.Collections.Generic;

    internal sealed class ReadOnlyDictionaryDiffBy<TKey, TValue> : ReadOnlyDictionaryDiffBy, IDiffBy
    {
        internal static readonly ReadOnlyDictionaryDiffBy<TKey, TValue> Default = new ReadOnlyDictionaryDiffBy<TKey, TValue>();

        private ReadOnlyDictionaryDiffBy()
        {
        }

        public void AddDiffs(
            DiffBuilder builder,
            object x,
            object y,
            MemberSettings settings)
        {
            AddDiffs(builder, (IReadOnlyDictionary<TKey, TValue>)x, (IReadOnlyDictionary<TKey, TValue>)y, settings);
        }

        private static void AddDiffs(
            DiffBuilder builder,
            IReadOnlyDictionary<TKey, TValue> x,
            IReadOnlyDictionary<TKey, TValue> y,
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
