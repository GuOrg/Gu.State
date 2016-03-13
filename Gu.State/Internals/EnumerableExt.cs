namespace Gu.State
{
    using System.Collections.Generic;

    internal static class EnumerableExt
    {
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
            Ensure.NotNull(source, nameof(source));
            return AppendIterator(source, element);
        }

        internal static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            Ensure.NotNull(source, nameof(source));
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
