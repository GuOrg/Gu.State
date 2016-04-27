namespace Gu.State
{
    using System;
    using System.Diagnostics;

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
                if (!Is.Enumerable(x, y))
                {
                    return;
                }

                Debug.Assert(settings.ReferenceHandling != ReferenceHandling.Throw, "Should not get here");

                IDiffBy comparer;
                if (ListDiffBy.TryGetOrCreate(x, y, out comparer) ||
                    ReadonlyListDiffBy.TryGetOrCreate(x, y, out comparer) ||
                    DictionaryDiffBy.TryGetOrCreate(x, y, out comparer) ||
                    ReadOnlyDictionaryDiffBy.TryGetOrCreate(x, y, out comparer) ||
                    SetDiffBy.TryGetOrCreate(x, y, out comparer) ||
                    EnumerableDiffBy.TryGetOrCreate(x, y, out comparer))
                {
                    comparer.AddDiffs(x, y, settings, collectionBuilder, itemDiff);
                    return;
                }

                throw Throw.ShouldNeverGetHereException("All enumarebles must be checked here");
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
        }
    }
}
