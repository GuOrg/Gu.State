namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    /// <inheritdoc />
    internal class DictionaryEqualByComparer<TKey, TValue> : EqualByComparer
    {
        public static readonly DictionaryEqualByComparer<TKey, TValue> Default = new DictionaryEqualByComparer<TKey, TValue>();

        private DictionaryEqualByComparer()
        {
        }

        /// <inheritdoc />
        public override bool Equals(
            object x,
            object y,
            MemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            if (TryGetEitherNullEquals(x, y, out bool result))
            {
                return result;
            }

            var xd = (IDictionary<TKey, TValue>)x;
            var yd = (IDictionary<TKey, TValue>)y;
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

            return KeysAndValuesEquals(xd, yd, settings, referencePairs);
        }

        private static bool KeysAndValuesEquals(
            IDictionary<TKey, TValue> x,
            IDictionary<TKey, TValue> y,
            MemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            foreach (var key in x.Keys)
            {
                var xv = x[key];

                if (!y.TryGetValue(key, out TValue yv))
                {
                    return false;
                }

                if (referencePairs?.Contains(xv, yv) == true)
                {
                    continue;
                }

                if (!EqualBy.MemberValues(xv, yv, settings, referencePairs))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool KeysAndValuesEquals(
            IDictionary<TKey, TValue> x,
            IDictionary<TKey, TValue> y,
            Func<TValue, TValue, bool> compareItem)
        {
            foreach (var key in x.Keys)
            {
                var xv = x[key];

                if (!y.TryGetValue(key, out TValue yv))
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