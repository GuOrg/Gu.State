namespace Gu.State
{
    using System.Collections.Generic;

    internal class DictionaryDiffBy<TKey, TValue> : DictionaryDiffBy, IDiffBy
    {
        public static readonly DictionaryDiffBy<TKey, TValue> Default = new DictionaryDiffBy<TKey, TValue>();

        private DictionaryDiffBy()
        {
        }

        public void AddDiffs(
            DiffBuilder collectionBuilder,
            object x,
            object y,
            IMemberSettings settings)
        {
            this.AddDiffs(collectionBuilder, (IDictionary<TKey, TValue>)x, (IDictionary<TKey, TValue>)y, settings);
        }

        private void AddDiffs<TSettings>(
            DiffBuilder collectionBuilder,
            IDictionary<TKey, TValue> x,
            IDictionary<TKey, TValue> y,
            TSettings settings)
                where TSettings : IMemberSettings
        {
            using (var borrow = HashSetPool<TKey>.Borrow(EqualityComparer<TKey>.Default.Equals, EqualityComparer<TKey>.Default.GetHashCode))
            {
                borrow.Value.UnionWith(x.Keys);
                borrow.Value.UnionWith(y.Keys);
                foreach (var key in borrow.Value)
                {
                    var xv = x.ElementAtOrMissing(key);
                    var yv = y.ElementAtOrMissing(key);
                    collectionBuilder.UpdateCollectionItemDiff(xv, yv, key, settings);
                }
            }
        }
    }
}