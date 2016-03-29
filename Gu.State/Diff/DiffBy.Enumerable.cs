namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    public static partial class DiffBy
    {
        private static class Enumerable
        {
            internal static List<Diff> Diffs<TSettings>(object x, object y, TSettings settings, ReferencePairCollection referencePairs, Func<object, object, TSettings, ReferencePairCollection, Diff> itemDiff)
                where TSettings : IMemberSettings
            {
                throw new NotImplementedException("message");
                //Debug.Assert(settings.ReferenceHandling != ReferenceHandling.Throw, "Should not get here");

                //var xl = x as IList;
                //var yl = y as IList;
                //if (xl != null && yl != null)
                //{
                //    return Diffs(xl, yl, itemDiff, settings, referencePairs);
                //}

                //if (xl != null || yl != null)
                //{
                //    return false;
                //}

                //var xd = x as IDictionary;
                //var yd = y as IDictionary;
                //if (xd != null && yd != null)
                //{
                //    return Diffs(xd, yd, itemDiff, settings, referencePairs);
                //}

                //if (xd != null || yd != null)
                //{
                //    return false;
                //}

                //var xe = x as IEnumerable;
                //var ye = y as IEnumerable;
                //if (xe != null && ye != null)
                //{
                //    return Diffs(xe, ye, itemDiff, settings, referencePairs);
                //}

                //if (xe != null || ye != null)
                //{
                //    return false;
                //}

                //var message = "There is a bug in the library as it:\r\n" +
                //              $"Could not compare enumerables of type {x.GetType().PrettyName()}";
                //throw new InvalidOperationException(message);
            }
        }
    }
}
