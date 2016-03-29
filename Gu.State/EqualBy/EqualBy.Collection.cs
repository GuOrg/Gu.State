namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Linq;

    public static partial class EqualBy
    {
        private static bool EnumerableEquals<TSetting>(
            object x,
            object y,
            Func<object, object, TSetting, ReferencePairCollection, bool> compareItem,
            TSetting settings,
            ReferencePairCollection referencePairs)
            where TSetting : class, IReferenceHandling
        {
            Debug.Assert(settings.ReferenceHandling != ReferenceHandling.Throw, "Should not get here");

            var xl = x as IList;
            var yl = y as IList;
            if (xl != null && yl != null)
            {
                return Collection.Equals(xl, yl, compareItem, settings, referencePairs);
            }

            if (xl != null || yl != null)
            {
                return false;
            }

            var xd = x as IDictionary;
            var yd = y as IDictionary;
            if (xd != null && yd != null)
            {
                return Collection.Equals(xd, yd, compareItem, settings, referencePairs);
            }

            if (xd != null || yd != null)
            {
                return false;
            }

            var xe = x as IEnumerable;
            var ye = y as IEnumerable;
            if (xe != null && ye != null)
            {
                return Collection.Equals(xe, ye, compareItem, settings, referencePairs);
            }

            if (xe != null || ye != null)
            {
                return false;
            }

            var message = "There is a bug in the library as it:\r\n" +
                          $"Could not compare enumerables of type {x.GetType().PrettyName()}";
            throw new InvalidOperationException(message);
        }

        private static class Collection
        {
            internal static bool Equals<TSetting>(
                IList x,
                IList y,
                Func<object, object, TSetting, ReferencePairCollection, bool> compareItem,
                TSetting settings,
                ReferencePairCollection referencePairs)
            {
                if (x.Count != y.Count)
                {
                    return false;
                }

                for (int i = 0; i < x.Count; i++)
                {
                    var xv = x[i];
                    var yv = y[i];
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

            internal static bool Equals<TSetting>(
                IDictionary x,
                IDictionary y,
                Func<object, object, TSetting, ReferencePairCollection, bool> compareItem,
                TSetting settings,
                ReferencePairCollection referencePairs)
            {
                if (x.Count != y.Count)
                {
                    return false;
                }

                foreach (var key in x.Keys)
                {
                    if (!y.Contains(key))
                    {
                        return false;
                    }

                    var xv = x[key];
                    var yv = y[key];
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

            internal static bool Equals<TSetting>(
                IEnumerable x,
                IEnumerable y,
                Func<object, object, TSetting, ReferencePairCollection, bool> compareItem,
                TSetting settings,
                ReferencePairCollection referencePairs)
            {
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
