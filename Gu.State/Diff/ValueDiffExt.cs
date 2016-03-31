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
                return new ValueDiff(source.X, source.Y, diffs);
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
    }
}