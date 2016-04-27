namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    /// <inheritdoc />
    public class ReadOnlyDictionaryEqualByComparer<TKey, TValue> : EqualByComparer
    {
        public static readonly ReadOnlyDictionaryEqualByComparer<TKey, TValue> Default = new ReadOnlyDictionaryEqualByComparer<TKey, TValue>();

        private ReadOnlyDictionaryEqualByComparer()
        {
        }

        /// <inheritdoc />
        public override bool Equals<TSetting>(
            object x,
            object y,
            Func<object, object, TSetting, ReferencePairCollection, bool> compareItem,
            TSetting settings,
            ReferencePairCollection referencePairs)
        {
            bool result;
            if (TryGetEitherNullEquals(x, y, out result))
            {
                return result;
            }

            var xd = (IReadOnlyDictionary<TKey, TValue>)x;
            var yd = (IReadOnlyDictionary<TKey, TValue>)y;
            if (xd.Count != yd.Count)
            {
                return false;
            }

            if (settings.IsEquatable(typeof(TValue)))
            {
                return KeysAndValuesEquals(xd, yd, EqualityComparer<TValue>.Default.Equals);
            }

            if (settings.ReferenceHandling == ReferenceHandling.References)
            {
                return KeysAndValuesEquals(xd, yd, (xi, yi) => ReferenceEquals(xi, yi));
            }

            return KeysAndValuesEquals(xd, yd, compareItem, settings, referencePairs);
        }

        internal static bool KeysAndValuesEquals<TSetting>(
            IReadOnlyDictionary<TKey, TValue> x,
            IReadOnlyDictionary<TKey, TValue> y,
            Func<object, object, TSetting, ReferencePairCollection, bool> compareItem,
            TSetting settings,
            ReferencePairCollection referencePairs)
        {
            foreach (var key in x.Keys)
            {
                var xv = x[key];

                TValue yv;
                if (!y.TryGetValue(key, out yv))
                {
                    return false;
                }

                if (referencePairs?.Contains(xv, yv) == true)
                {
                    continue;
                }

                if (!compareItem(xv, yv, settings, referencePairs))
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool KeysAndValuesEquals(
            IReadOnlyDictionary<TKey, TValue> x,
            IReadOnlyDictionary<TKey, TValue> y,
            Func<TValue, TValue, bool> compareItem)
        {
            foreach (var key in x.Keys)
            {
                var xv = x[key];

                TValue yv;
                if (!y.TryGetValue(key, out yv))
                {
                    return false;
                }

                if (!compareItem(xv, yv))
                {
                    return false;
                }
            }

            return true;
        }
    }
}