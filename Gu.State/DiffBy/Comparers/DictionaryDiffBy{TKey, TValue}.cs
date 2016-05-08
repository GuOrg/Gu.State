namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal class DictionaryDiffBy<TKey, TValue> : DictionaryDiffBy, IDiffBy
    {
        public static readonly DictionaryDiffBy<TKey, TValue> Default = new DictionaryDiffBy<TKey, TValue>();

        private DictionaryDiffBy()
        {
        }

        public void AddDiffs<TSettings>(
            DiffBuilder collectionBuilder,
            object x,
            object y,
            TSettings settings,
            Action<DiffBuilder, object, object, object, TSettings> itemDiff)
            where TSettings : IMemberSettings
        {
            this.AddDiffs(collectionBuilder, (IDictionary<TKey, TValue>)x, (IDictionary<TKey, TValue>)y, settings, itemDiff);
        }

        private void AddDiffs<TSettings>(
            DiffBuilder collectionBuilder,
            IDictionary<TKey, TValue> x,
            IDictionary<TKey, TValue> y,
            TSettings settings,
            Action<DiffBuilder, object, object, object, TSettings> itemDiff)
                where TSettings : IMemberSettings
        {
            using (var borrow = SetPool<TKey>.Borrow(EqualityComparer<TKey>.Default.Equals, EqualityComparer<TKey>.Default.GetHashCode))
            {
                borrow.Value.UnionWith(x.Keys);
                borrow.Value.UnionWith(y.Keys);
                foreach (var key in borrow.Value)
                {
                    var xv = x.ElementAtOrMissing(key);
                    var yv = y.ElementAtOrMissing(key);
                    itemDiff(collectionBuilder, xv, yv, key, settings);
                }
            }
        }
    }
}