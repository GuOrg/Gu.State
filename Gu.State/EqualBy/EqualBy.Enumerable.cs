namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Diagnostics;

    public static partial class EqualBy
    {
        private static bool EnumerableEquals<TSetting>(
            object x,
            object y,
            Func<object, object, TSetting, ReferencePairCollection, bool> compareItem,
            TSetting settings,
            ReferencePairCollection referencePairs)
            where TSetting : class, IMemberSettings
        {
            Debug.Assert(settings.ReferenceHandling != ReferenceHandling.Throw, "Should not get here");

            EqualByComparer comparer;
            if (ListEqualByComparer.TryGetOrCreate(x, y, out comparer) ||
                SetEqualByComparer.TryGetOrCreate(x, y, out comparer) ||
                ArrayEqualByComparer.TryGetOrCreate(x, y, out comparer) ||
                DictionaryEqualByComparer.TryGetOrCreate(x, y, out comparer) ||
                EnumerableEqualByComparer.TryGetOrCreate(x, y, out comparer))
            {
                return comparer.Equals(x, y, compareItem, settings, referencePairs);
            }

            var message = "There is a bug in the library as it:\r\n" +
                          $"Could not compare enumerables of type {x.GetType().PrettyName()}";
            throw new InvalidOperationException(message);
        }

        private static class Collection
        {
            internal static bool Equals<TSetting>(
                IEnumerable x,
                IEnumerable y,
                Func<object, object, TSetting, ReferencePairCollection, bool> compareItem,
                TSetting settings,
                ReferencePairCollection referencePairs)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                foreach (var pair in new PaddedPairs(x, y))
                {
                    if (referencePairs?.Contains(pair.X, pair.Y) == true)
                    {
                        continue;
                    }

                    if (!compareItem(pair.X, pair.Y, settings, referencePairs))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
