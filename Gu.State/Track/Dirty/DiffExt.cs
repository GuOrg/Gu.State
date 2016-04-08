namespace Gu.State
{
    using System;
    using System.Collections.Generic;
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

        internal static ValueDiff With(this ValueDiff source, object x, object y, PropertyInfo propertyInfo, ValueDiff propertyValueDiff)
        {
            if (source == null)
            {
                var propertyDiff = new PropertyDiff(propertyInfo, propertyValueDiff);
                return new ValueDiff(x, y, new[] { propertyDiff });
            }

            var diffs = source.Diffs.ReplaceOdAdd(diff => IsPropertyMatch(diff, propertyInfo), new PropertyDiff(propertyInfo, propertyValueDiff));
            return new ValueDiff(x, y, diffs);
        }

        internal static ValueDiff With(this ValueDiff source, object x, object y, int index, ValueDiff indexValueDiff)
        {
            if (source == null)
            {
                var indexDiff = new IndexDiff(index, indexValueDiff);
                return new ValueDiff(x, y, new[] { indexDiff });
            }

            var diffs = source.Diffs.ReplaceOdAdd(diff => IsIndexMatch(diff, index), new IndexDiff(index, indexValueDiff));
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

        private static IReadOnlyList<SubDiff> ReplaceOdAdd(this IReadOnlyList<SubDiff> source, Func<SubDiff, bool> isMatch, SubDiff newDiff)
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
            foreach (var item in source.Diffs)
            {
                if (isMatch(item))
                {
                    continue;
                }

                result.Add(item);
            }

            return result;
        }
    }
}