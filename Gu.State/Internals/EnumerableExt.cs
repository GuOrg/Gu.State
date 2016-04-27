namespace Gu.State
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal static class EnumerableExt
    {
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

        // http://stackoverflow.com/a/969118/1069200
        internal static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source)
        {
            T previous = default(T);
            bool first = true;
            foreach (T element in source)
            {
                if (!first)
                {
                    yield return previous;
                }

                previous = element;
                first = false;
            }
        }

        internal static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            Debug.Assert(source != null, "source == null");
            return AppendIterator(source, element);
        }

        internal static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            Debug.Assert(source != null, "source == null");
            return PrependIterator(source, element);
        }

        private static IEnumerable<TSource> AppendIterator<TSource>(IEnumerable<TSource> source, TSource element)
        {
            foreach (TSource e1 in source)
            {
                yield return e1;
            }

            yield return element;
        }

        private static IEnumerable<TSource> PrependIterator<TSource>(IEnumerable<TSource> source, TSource element)
        {
            yield return element;

            foreach (TSource e1 in source)
            {
                yield return e1;
            }
        }
    }
}
