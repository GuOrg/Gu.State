namespace Gu.State
{
    using System.Collections;
    using System.Collections.Generic;

    internal static class ListExt
    {
        internal static void SetElementAt<T>(this IList<T> list, int index, T item)
        {
            if (index >= 0 && index < list.Count)
            {
                list[index] = item;
            }
            else if (index == list.Count)
            {
                list.Add(item);
            }
            else
            {
                throw Throw.ShouldNeverGetHereException($"Trying to set index: {index} when length is {list.Count}");
            }
        }

        internal static void TrimLengthTo(this IList source, IList target)
        {
            source.TrimLengthTo(target.Count);
        }

        internal static void TrimLengthTo<T>(this IList<T> source, IList<T> target)
        {
            source.TrimLengthTo(target.Count);
        }

        internal static void TrimLengthTo<T>(this List<T> source, int targetCount)
        {
            ((IList<T>)source).TrimLengthTo(targetCount);
        }

        internal static void TrimLengthTo(this IList source, int targetCount)
        {
            for (var i = source.Count - 1; i >= targetCount; i--)
            {
                source.RemoveAt(i);
            }
        }

        internal static void TrimLengthTo<T>(this IList<T> source, int targetCount)
        {
            for (var i = source.Count - 1; i >= targetCount; i--)
            {
                source.RemoveAt(i);
            }
        }
    }
}
