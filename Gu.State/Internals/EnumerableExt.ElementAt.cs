namespace Gu.State
{
    using System.Collections;
    using System.Collections.Generic;

    internal static partial class EnumerableExt
    {
        internal static object ElementAtOrDefault(this IList list, int index)
        {
            if (index < list.Count)
            {
                return list[index];
            }

            return null;
        }

        internal static T ElementAtOrDefault<T>(this IList<T> list, int index)
        {
            if (index < list.Count)
            {
                return list[index];
            }

            return default(T);
        }

        internal static object ElementAtOrDefault(this IDictionary dictionary, object key)
        {
            if (dictionary.Contains(key))
            {
                return dictionary[key];
            }

            return null;
        }

        internal static object ElementAtOrMissing(this IList list, int index)
        {
            if (index < list.Count)
            {
                return list[index];
            }

            return PaddedPairs.MissingItem;
        }

        internal static object ElementAtOrMissing<T>(this IList<T> list, int index)
        {
            if (index < list.Count)
            {
                return list[index];
            }

            return PaddedPairs.MissingItem;
        }

        internal static object ElementAtOrMissing<T>(this IReadOnlyList<T> list, int index)
        {
            if (index < list.Count)
            {
                return list[index];
            }

            return PaddedPairs.MissingItem;
        }

        internal static object ElementAtOrMissing(this IDictionary dictionary, object key)
        {
            if (dictionary.Contains(key))
            {
                return dictionary[key];
            }

            return PaddedPairs.MissingItem;
        }

        internal static object ElementAtOrMissing<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                return value;
            }

            return PaddedPairs.MissingItem;
        }

        internal static object ElementAtOrMissing<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                return value;
            }

            return PaddedPairs.MissingItem;
        }

    }
}