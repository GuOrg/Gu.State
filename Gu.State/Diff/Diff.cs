namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class Diff
    {
        protected static readonly Diff[] EmptyDiffs = new Diff[0];
        public static readonly Diff Empty = new Diff(EmptyDiffs);

        internal Diff(IReadOnlyCollection<Diff> diffs)
        {
            this.Diffs = diffs;
        }

        public IReadOnlyCollection<Diff> Diffs { get; }

        public virtual bool IsEmpty => this.Diffs.Count == 0;

        public Diff Without(PropertyInfo propertyInfo)
        {
            if (this.Diffs.OfType<PropertyDiff>().Any(x => x.PropertyInfo == propertyInfo))
            {
                return new Diff(this.Diffs.Where(x => (x as PropertyDiff)?.PropertyInfo != propertyInfo).ToArray());
            }

            return this;
        }

        public Diff With(PropertyInfo propertyInfo, object xValue, object yValue)
        {
            if (this.Diffs.OfType<PropertyDiff>().Any(x => x.PropertyInfo == propertyInfo))
            {
                var diffs = this.Diffs.Where(x => (x as PropertyDiff)?.PropertyInfo != propertyInfo).Append(new PropertyDiff(propertyInfo, xValue, yValue)).ToArray();
                return new Diff(diffs);
            }

            return new Diff(this.Diffs.Append(new PropertyDiff(propertyInfo, xValue, yValue)).ToArray());
        }

        public override string ToString()
        {
            if (this.IsEmpty)
            {
                return string.Empty;
            }

            throw new NotImplementedException();
        }
    }
}
