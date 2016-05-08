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

        internal static bool TryTrimLengthTo(this IList source, IList target)
        {
            return source.TryTrimLengthTo(target.Count);
        }

        internal static bool TryTrimLengthTo<T>(this IList<T> source, IList<T> target)
        {
            return source.TryTrimLengthTo(target.Count);
        }

        internal static bool TryTrimLengthTo(this IList source, int targetCount)
        {
            var trimmed = source.Count > targetCount;
            for (var i = source.Count - 1; i >= targetCount; i--)
            {
                source.RemoveAt(i);
            }

            return trimmed;
        }

        internal static bool TryTrimLengthTo<T>(this IList<T> source, int targetCount)
        {
            var trimmed = source.Count > targetCount;
            for (var i = source.Count - 1; i >= targetCount; i--)
            {
                source.RemoveAt(i);
            }

            return trimmed;
        }
    }
}
