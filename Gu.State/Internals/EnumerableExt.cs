namespace Gu.State
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal static partial class EnumerableExt
    {
        // http://stackoverflow.com/a/969118/1069200
        internal static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source)
        {
            var previous = default(T);
            var first = true;
            foreach (var element in source)
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
            foreach (var e1 in source)
            {
                yield return e1;
            }

            yield return element;
        }

        private static IEnumerable<TSource> PrependIterator<TSource>(IEnumerable<TSource> source, TSource element)
        {
            yield return element;

            foreach (var e1 in source)
            {
                yield return e1;
            }
        }
    }
}
