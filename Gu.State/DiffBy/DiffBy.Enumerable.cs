namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public static partial class DiffBy
    {
        private static class Enumerable
        {
            internal static List<Diff> Diffs<TSettings>(
                object x,
                object y,
                TSettings settings,
                ReferencePairCollection referencePairs,
                Func<object, object, TSettings, ReferencePairCollection, ValueDiff> itemDiff)
                where TSettings : IMemberSettings
            {
                if (!(x is IEnumerable) || !(y is IEnumerable))
                {
                    return null;
                }

                Debug.Assert(settings.ReferenceHandling != ReferenceHandling.Throw, "Should not get here");

                IList xl;
                IList yl;
                if (Try.CastAs(x, y, out xl, out yl))
                {
                    return Diffs(xl, yl, settings, referencePairs, itemDiff);
                }

                IDictionary xd;
                IDictionary yd;
                if (Try.CastAs(x, y, out xd, out yd))
                {
                    return Diffs(xd, yd, settings, referencePairs, itemDiff);
                }

                if (Is.Sets(x, y))
                {
                    var xe = Set.ItemsOrderByHashCode(x);
                    var ye = Set.ItemsOrderByHashCode(y);
                    if (xe.HasCollision || ye.HasCollision)
                    {
                        throw new NotImplementedException("message");
                    }
                    else
                    {
                        return Diffs(xe, ye, settings, referencePairs, itemDiff);
                    }
                }

                return Diffs((IEnumerable)x, (IEnumerable)y, settings, referencePairs, itemDiff);
            }

            private static List<Diff> Diffs<TSettings>(
                IList x,
                IList y,
                TSettings settings,
                ReferencePairCollection referencePairs,
                Func<object, object, TSettings, ReferencePairCollection, ValueDiff> itemDiff)
                where TSettings : IMemberSettings
            {
                return Diffs((IEnumerable)x, (IEnumerable)y, settings, referencePairs, itemDiff);
            }

            private static List<Diff> Diffs<TSettings>(
                IDictionary x,
                IDictionary y,
                TSettings settings,
                ReferencePairCollection referencePairs,
                Func<object, object, TSettings, ReferencePairCollection, ValueDiff> itemDiff)
                where TSettings : IMemberSettings
            {
                if (x == null || y == null)
                {
                    throw Throw.ShouldNeverGetHereException("should be checked for same type before");
                }

                List<Diff> diffs = null;
                foreach (var key in x.Keys.OfType<object>().Concat(y.Keys.OfType<object>()).Distinct())
                {
                    IndexDiff diff = null;
                    if (!x.Contains(key))
                    {
                        diff = new IndexDiff(key, PaddedPairs.MissingItem, y[key]);
                    }
                    else if (!y.Contains(key))
                    {
                        diff = new IndexDiff(key, x[key], PaddedPairs.MissingItem);
                    }
                    else
                    {
                        var xv = x[key];
                        var yv = y[key];
                        if (referencePairs?.Contains(xv, yv) == true)
                        {
                            continue;
                        }

                        var id = itemDiff(xv, yv, settings, referencePairs);
                        if (id != null)
                        {
                            diff = new IndexDiff(key, id);
                        }
                    }

                    if (diff != null)
                    {
                        if (diffs == null)
                        {
                            diffs = new List<Diff>();
                        }

                        diffs.Add(diff);
                    }
                }

                return diffs;
            }

            private static List<Diff> Diffs<TSettings>(
                IEnumerable x,
                IEnumerable y,
                TSettings settings,
                ReferencePairCollection referencePairs,
                Func<object, object, TSettings, ReferencePairCollection, ValueDiff> itemDiff)
                where TSettings : IMemberSettings
            {
                if (x == null || y == null)
                {
                    throw Throw.ShouldNeverGetHereException("should be checked for same type before");
                }

                var i = -1;
                List<Diff> diffs = null;
                foreach (var pair in new PaddedPairs(x, y))
                {
                    i++;
                    if (referencePairs?.Contains(pair.X, pair.Y) == true)
                    {
                        continue;
                    }

                    var diff = itemDiff(pair.X, pair.Y, settings, referencePairs);
                    if (diff != null)
                    {
                        if (diffs == null)
                        {
                            diffs = new List<Diff>();
                        }

                        diffs.Add(new IndexDiff(i, diff));
                    }
                }

                return diffs;
            }
        }
    }
}
