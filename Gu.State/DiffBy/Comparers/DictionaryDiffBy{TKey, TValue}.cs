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
            object x,
            object y,
            TSettings settings,
            DiffBuilder collectionBuilder,
            Action<object, object, object, TSettings, DiffBuilder> itemDiff)
            where TSettings : IMemberSettings
        {
            this.AddDiffs((IDictionary<TKey, TValue>)x, (IDictionary<TKey, TValue>)y, settings, collectionBuilder, itemDiff);
        }

        private void AddDiffs<TSettings>(
            IDictionary<TKey, TValue> x,
            IDictionary<TKey, TValue> y,
            TSettings settings,
            DiffBuilder collectionBuilder,
            Action<object, object, object, TSettings, DiffBuilder> itemDiff)
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
                    itemDiff(xv, yv, key, settings, collectionBuilder);
                }
            }
        }
    }
}