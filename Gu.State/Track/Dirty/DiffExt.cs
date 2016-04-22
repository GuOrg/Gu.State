namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    internal static class DiffExt
    {
        internal static ValueDiff Without(this ValueDiff source, PropertyInfo propertyInfo)
        {
            if (source == null)
            {
                return null;
            }

            var diffs = RemoveDiff(source, diff => IsPropertyMatch(diff, propertyInfo));
            return diffs.Count == 0
                       ? null
                       : new ValueDiff(source.X, source.Y, diffs);
        }

        internal static ValueDiff Without(this ValueDiff source, int index)
        {
            if (source == null)
            {
                return null;
            }

            var diffs = RemoveDiff(source, diff => IsIndexMatch(diff, index));
            return diffs.Count == 0
                       ? null
                       : new ValueDiff(source.X, source.Y, diffs);
        }

        internal static ValueDiff With(
            this ValueDiff source,
            object x,
            object y,
            PropertyInfo propertyInfo,
            ValueDiff propertyValueDiff)
        {
            if (source == null)
            {
                var propertyDiff = new PropertyDiff(propertyInfo, propertyValueDiff);
                return new ValueDiff(x, y, new[] { propertyDiff });
            }

            var diffs = source.Diffs.ReplaceOdAdd(
                diff => IsPropertyMatch(diff, propertyInfo),
                new PropertyDiff(propertyInfo, propertyValueDiff));
            return new ValueDiff(x, y, diffs);
        }

        internal static ValueDiff With(this ValueDiff source, object x, object y, int index, ValueDiff indexValueDiff)
        {
            if (source == null)
            {
                var indexDiff = new IndexDiff(index, indexValueDiff);
                return new ValueDiff(x, y, new[] { indexDiff });
            }

            var diffs = source.Diffs.ReplaceOdAdd(
                diff => IsIndexMatch(diff, index),
                new IndexDiff(index, indexValueDiff));
            return new ValueDiff(x, y, diffs);
        }

        private static bool IsIndexMatch(Diff diff, int index)
        {
            var indexDiff = diff as IndexDiff;
            if (indexDiff == null)
            {
                return false;
            }

            return (int)indexDiff.Index == index;
        }

        private static bool IsPropertyMatch(Diff diff, PropertyInfo propertyInfo)
        {
            var propertyDiff = diff as PropertyDiff;
            if (propertyDiff == null)
            {
                return false;
            }

            return propertyDiff.PropertyInfo == propertyInfo;
        }

        private static IReadOnlyList<SubDiff> ReplaceOdAdd(
            this IReadOnlyList<SubDiff> source,
            Func<SubDiff, bool> isMatch,
            SubDiff newDiff)
        {
            var result = new List<SubDiff>(source.Count);
            bool replaced = false;
            foreach (var item in source)
            {
                if (isMatch(item))
                {
                    replaced = true;
                    result.Add(newDiff);
                    continue;
                }

                result.Add(item);
            }

            if (!replaced)
            {
                result.Add(newDiff);
            }

            return result;
        }

        private static IReadOnlyList<SubDiff> RemoveDiff(this ValueDiff source, Func<SubDiff, bool> isMatch)
        {
            var result = new List<SubDiff>(source.Diffs.Count);
            SubDiff match = null;
            foreach (var subDiff in source.Diffs)
            {
                if (isMatch(subDiff))
                {
                    match = subDiff;
                    continue;
                }

                result.Add(subDiff);
            }

            return RemoveRecursiveMatches(result, match, isMatch);
        }

        private static IReadOnlyList<SubDiff> RemoveRecursiveMatches(List<SubDiff> diffs, SubDiff removed, Func<SubDiff, bool> isMatch)
        {
            if (removed == null || diffs.Count != 1)
            {
                return diffs;
            }

            using (var disposer = Diff.BorrowReferenceList())
            {
                throw new NotImplementedException("message");
                //var singles = SingleItemDiffs(diffs[0], disposer.Value);
                //if (singles != null)
                //{
                //    var node = singles[singles.Count - 1];
                //    if (isMatch(node) && Equals(removed.X, node.X) && Equals(removed.Y, node.Y))
                //    {
                //        diffs.Clear();
                //    }
                //}
            }

            return diffs;
        }

        private static HashSet<SubDiff> SingleItemDiffs(SubDiff diff, HashSet<SubDiff> diffs)
        {
            if (diffs == null)
            {
                return null;
            }

            if (diff.Diffs.Count == 0)
            {
                diffs.Add(diff);
                return diffs;
            }

            if (diff.Diffs.Count != 1)
            {
                return null;
            }

            if (diffs.Contains(diff))
            {
                diffs.Add(diff.Diffs[0]);
                return diffs;
            }

            diffs.Add(diff);
            diffs = SingleItemDiffs(diff.Diffs[0], diffs);
            return diffs;
        }
    }
}