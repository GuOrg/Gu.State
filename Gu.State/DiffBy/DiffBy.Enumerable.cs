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
            internal static void AddItemDiffs<TSettings>(
                object x,
                object y,
                TSettings settings,
                DiffBuilder collectionBuilder,
                Action<object, object, object, TSettings, DiffBuilder> itemDiff)
                where TSettings : IMemberSettings
            {
                if (!(x is IEnumerable) || !(y is IEnumerable))
                {
                    return;
                }

                Debug.Assert(settings.ReferenceHandling != ReferenceHandling.Throw, "Should not get here");

                IList xl;
                IList yl;
                if (Try.CastAs(x, y, out xl, out yl))
                {
                    Diffs(xl, yl, settings, collectionBuilder, itemDiff);
                    return;
                }

                IDictionary xd;
                IDictionary yd;
                if (Try.CastAs(x, y, out xd, out yd))
                {
                    Diffs(xd, yd, settings, collectionBuilder, itemDiff);
                    return;
                }

                if (Is.Sets(x, y))
                {
                    var xe = Set.ItemsOrderByHashCode(x);
                    var ye = Set.ItemsOrderByHashCode(y);
                    Diffs(xe, ye, settings, collectionBuilder, itemDiff);
                    return;
                }

                Diffs((IEnumerable)x, (IEnumerable)y, settings, collectionBuilder, itemDiff);
            }

            private static void Diffs<TSettings>(
                IList x,
                IList y,
                TSettings settings,
                DiffBuilder collectionBuilder,
                Action<object, object, object, TSettings, DiffBuilder> itemDiff)
                where TSettings : IMemberSettings
            {
                for (int i = 0; i < Math.Max(x.Count, y.Count); i++)
                {
                    var xv = x.ElementAtOrMissing(i);
                    var yv = y.ElementAtOrMissing(i);
                    itemDiff(xv, yv, i, settings, collectionBuilder);
                }
            }

            private static void Diffs<TSettings>(
                IDictionary x,
                IDictionary y,
                TSettings settings,
                DiffBuilder collectionBuilder,
                Action<object, object, object, TSettings, DiffBuilder> itemDiff)
                where TSettings : IMemberSettings
            {
                if (x == null || y == null)
                {
                    throw Throw.ShouldNeverGetHereException("should be checked for same type before");
                }

                foreach (var key in x.Keys.OfType<object>().Concat(y.Keys.OfType<object>()).Distinct())
                {
                    var xv = x.ElementAtOrMissing(key);
                    var yv = y.ElementAtOrMissing(key);
                    itemDiff(xv, yv, key, settings, collectionBuilder);
                }
            }

            private static void Diffs<TSettings>(
                Set.ISortedByHashCode xSorted,
                Set.ISortedByHashCode ySorted,
                TSettings settings,
                DiffBuilder collectionBuilder,
                Action<object, object, object, TSettings, DiffBuilder> itemDiff)
                 where TSettings : IMemberSettings
            {
                throw new NotImplementedException("message");

                //for (int xi = xSorted.Count - 1; xi >= 0; xi--)
                //{
                //    var xItem = xSorted[xi];
                //    bool found = false;
                //    var indices = ySorted.MatchingHashIndices(xItem);
                //    if (indices.IsNone)
                //    {
                //        if (diffs == null)
                //        {
                //            diffs = new List<SubDiff>();
                //        }

                //        diffs.Add(new IndexDiff(xItem, new ValueDiff(xItem, PaddedPairs.MissingItem)));
                //        continue;
                //    }

                //    ValueDiff valueDiff = null;
                //    for (int yi = indices.First; yi <= indices.Last; yi++)
                //    {
                //        var yItem = ySorted[yi];
                //        valueDiff = itemDiff(xItem, yItem, settings, referencePairs);
                //        if (valueDiff == null)
                //        {
                //            found = true;
                //            xSorted.RemoveAt(xi);
                //            ySorted.RemoveAt(yi);
                //            break;
                //        }
                //    }

                //    if (!found)
                //    {
                //        if (diffs == null)
                //        {
                //            diffs = new List<SubDiff>();
                //        }

                //        diffs.Add(new IndexDiff(xItem, valueDiff));
                //        xSorted.RemoveAt(xi);
                //        ySorted.RemoveAt(indices.Last);
                //    }
                //}

                //foreach (var yItem in ySorted)
                //{
                //    if (diffs == null)
                //    {
                //        diffs = new List<SubDiff>();
                //    }

                //    diffs.Add(new IndexDiff(yItem, new ValueDiff(PaddedPairs.MissingItem, yItem)));
                //}

                //return diffs;
            }

            private static void Diffs<TSettings>(
                IEnumerable x,
                IEnumerable y,
                TSettings settings,
                DiffBuilder collectionBuilder,
                Action<object, object, object, TSettings, DiffBuilder> itemDiff)
                where TSettings : IMemberSettings
            {
                if (x == null || y == null)
                {
                    throw Throw.ShouldNeverGetHereException("should be checked for same type before");
                }

                var i = -1;
                foreach (var pair in new PaddedPairs(x, y))
                {
                    i++;
                    itemDiff(pair.X, pair.Y, i, settings, collectionBuilder);
                }
            }
        }
    }
}
