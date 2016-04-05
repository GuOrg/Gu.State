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
            if (ListEqualByComparer.TryGetOrCreate(x, y, out comparer))
            {
                return comparer.Equals(x, y, compareItem, settings, referencePairs);
            }

            if (x is Array && y is Array)
            {
                return ArrayEqualByComparer.Default.Equals(x, y, compareItem, settings, referencePairs);
            }

            IDictionary xd;
            IDictionary yd;
            if (Try.CastAs(x, y, out xd, out yd))
            {
                return Collection.Equals(xd, yd, compareItem, settings, referencePairs);
            }

            if (Is.Sets(x, y))
            {
                return Collection.SetEquals(x, y, compareItem, settings, referencePairs);
            }

            IEnumerable xe;
            IEnumerable ye;
            if (Try.CastAs(x, y, out xe, out ye))
            {
                return Collection.Equals(xe, ye, compareItem, settings, referencePairs);
            }

            var message = "There is a bug in the library as it:\r\n" +
                          $"Could not compare enumerables of type {x.GetType().PrettyName()}";
            throw new InvalidOperationException(message);
        }

        private static class Collection
        {
            internal static bool Equals<TSetting>(
                IDictionary x,
                IDictionary y,
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

            internal static bool SetEquals<TSetting>(
                object x,
                object y,
                Func<object, object, TSetting, ReferencePairCollection, bool> compareItem,
                TSetting settings,
                ReferencePairCollection referencePairs)
                where TSetting : IMemberSettings
            {
                if (!Set.Equals(x, y))
                {
                    return false;
                }

                var xe = Set.ItemsOrderByHashCode(x);
                var ye = Set.ItemsOrderByHashCode(y);
                for (int xi = xe.Count - 1; xi >= 0; xi--)
                {
                    var xItem = xe[xi];
                    bool found = false;
                    var indices = ye.MatchingHashIndices(xItem);
                    if (indices.IsNone)
                    {
                        return false;
                    }

                    for (int yi = indices.First; yi <= indices.Last; yi++)
                    {
                        var yItem = ye[yi];
                        if (compareItem(xItem, yItem, settings, referencePairs))
                        {
                            found = true;
                            xe.RemoveAt(xi);
                            ye.RemoveAt(yi);
                            break;
                        }
                    }

                    if (!found)
                    {
                        return false;
                    }
                }

                return xe.Count == 0 && ye.Count == 0;
            }

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
