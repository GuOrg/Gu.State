namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    /// <inheritdoc />
    internal class ReadOnlyDictionaryEqualByComparer<TKey, TValue> : EqualByComparer
    {
        public static readonly ReadOnlyDictionaryEqualByComparer<TKey, TValue> Default = new ReadOnlyDictionaryEqualByComparer<TKey, TValue>();

        private ReadOnlyDictionaryEqualByComparer()
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

            return KeysAndValuesEquals(xd, yd, settings, referencePairs);
        }

        internal static bool KeysAndValuesEquals(
            IReadOnlyDictionary<TKey, TValue> x,
            IReadOnlyDictionary<TKey, TValue> y,
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

        internal static bool KeysAndValuesEquals(
            IReadOnlyDictionary<TKey, TValue> x,
            IReadOnlyDictionary<TKey, TValue> y,
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