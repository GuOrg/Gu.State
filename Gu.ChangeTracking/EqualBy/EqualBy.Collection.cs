namespace Gu.ChangeTracking
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
    where TSetting : IEqualBySettings
        {
            Debug.Assert(settings.ReferenceHandling != ReferenceHandling.Throw, "Should not get here");

            var xl = x as IList;
            var yl = y as IList;
            if (xl != null && yl != null)
            {
                if (xl.Count != yl.Count)
                {
                    return false;
                }

                for (int i = 0; i < xl.Count; i++)
                {
                    var xv = xl[i];
                    var yv = yl[i];
                    if (referencePairs?.Contains(xv, yv) == true)
                    {
                        continue;
                    }
                    if (!compareItem(xv, yv, settings, referencePairs))
                    {
                        return false;
                    }
                }
            }
            else if (xl != null || yl != null)
            {
                return false;
            }
            else
            {
                var xe = x as IEnumerable;
                var ye = y as IEnumerable;
                if (xe != null && ye != null)
                {
                    foreach (var pair in new PaddedPairs(xe, ye))
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
                }
                else if (xe != null || ye != null)
                {
                    return false;
                }
                else
                {
                    var message = "There is a bug in the library as it:\r\n" +
                                  $"Could not compare enumerables of type {x.GetType().PrettyName()}";
                    throw new InvalidOperationException(message);
                }
            }

            return true;
        }

    }
}
