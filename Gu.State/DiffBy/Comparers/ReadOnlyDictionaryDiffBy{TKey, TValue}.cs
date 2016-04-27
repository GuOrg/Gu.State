namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal class ReadOnlyDictionaryDiffBy<TKey, TValue> : ReadOnlyDictionaryDiffBy, IDiffBy
    {
        public static readonly ReadOnlyDictionaryDiffBy<TKey, TValue> Default = new ReadOnlyDictionaryDiffBy<TKey, TValue>();

        private ReadOnlyDictionaryDiffBy()
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
            this.AddDiffs((IReadOnlyDictionary<TKey, TValue>)x, (IReadOnlyDictionary<TKey, TValue>)y, settings, collectionBuilder, itemDiff);
        }

        private void AddDiffs<TSettings>(
            IReadOnlyDictionary<TKey, TValue> x,
            IReadOnlyDictionary<TKey, TValue> y,
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