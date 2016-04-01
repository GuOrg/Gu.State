namespace Gu.State
{
    using System.Linq;
    using System.Reflection;

    internal static class ValueDiffExt
    {
        internal static ValueDiff Without(this ValueDiff source, PropertyInfo propertyInfo)
        {
            if (source == null)
            {
                return null;
            }

            if (source.Diffs.OfType<PropertyDiff>()
                    .Any(x => x.PropertyInfo == propertyInfo))
            {
                var diffs = source.Diffs.Except<Diff, PropertyDiff>(x => x.PropertyInfo == propertyInfo)
                                .ToArray();
                return diffs.Any()
                           ? new ValueDiff(source.X, source.Y, diffs)
                           : null;
            }

            return source;
        }

        internal static ValueDiff Without(this ValueDiff source, int index)
        {
            if (source == null)
            {
                return null;
            }

            if (source.Diffs.OfType<IndexDiff>()
                    .Any(x => (int)x.Index == index))
            {
                var diffs = source.Diffs.Except<Diff, IndexDiff>(x => (int)x.Index == index)
                                  .ToArray();
                return diffs.Any()
                           ? new ValueDiff(source.X, source.Y, diffs)
                           : null;
            }

            return source;
        }

        internal static ValueDiff With(this ValueDiff source, object x, object y, PropertyInfo propertyInfo, ValueDiff propertyValueDiff)
        {
            var propertyDiff = new PropertyDiff(propertyInfo, propertyValueDiff);

            if (source == null)
            {
                return new ValueDiff(x, y, new Diff[] { propertyDiff });
            }

            var diffs = source.Diffs.Except<Diff, PropertyDiff>(d => d.PropertyInfo == propertyInfo)
                              .Append(propertyDiff)
                              .ToArray();

            return new ValueDiff(source.X, source.Y, diffs);
        }

        internal static ValueDiff With(this ValueDiff source, object x, object y, int index, ValueDiff indexValueDiff)
        {
            var propertyDiff = new IndexDiff(index, indexValueDiff);

            if (source == null)
            {
                return new ValueDiff(x, y, new Diff[] { propertyDiff });
            }

            var diffs = source.Diffs.Except<Diff, IndexDiff>(d => (int)d.Index == index)
                              .Append(propertyDiff)
                              .ToArray();

            return new ValueDiff(source.X, source.Y, diffs);
        }
    }
}