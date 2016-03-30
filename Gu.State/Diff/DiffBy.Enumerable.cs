namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

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
                Debug.Assert(settings.ReferenceHandling != ReferenceHandling.Throw, "Should not get here");
                if (!(x is IEnumerable) || !(y is IEnumerable))
                {
                    return null;
                }

                var xl = x as IList;
                var yl = y as IList;
                if (xl != null && yl != null)
                {
                    return Diffs(xl, yl, settings, referencePairs, itemDiff);
                }

                if (xl != null || yl != null)
                {
                    throw Throw.ShouldNeverGetHereException("should be checked for same type before");
                }

                var xd = x as IDictionary;
                var yd = y as IDictionary;
                if (xd != null && yd != null)
                {
                    return Diffs(xd, yd, settings, referencePairs, itemDiff);
                }

                if (xd != null || yd != null)
                {
                    throw Throw.ShouldNeverGetHereException("should be checked for same type before");
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
                throw new NotImplementedException();
            }

            private static List<Diff> Diffs<TSettings>(
                IEnumerable x,
                IEnumerable y,
                TSettings settings,
                ReferencePairCollection referencePairs,
                Func<object, object, TSettings, ReferencePairCollection, ValueDiff> itemDiff)
                where TSettings : IMemberSettings
            {
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
