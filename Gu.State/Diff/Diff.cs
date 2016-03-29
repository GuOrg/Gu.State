namespace Gu.State
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    public abstract class Diff
    {
        private static readonly IReadOnlyCollection<Diff> Empty = new Diff[0];

        internal Diff(IReadOnlyCollection<Diff> diffs = null)
        {
            this.Diffs = diffs ?? Empty;
        }

        public IReadOnlyCollection<Diff> Diffs { get; }

        public abstract string ToString(string tabString, string newLine);

        internal abstract IndentedTextWriter WriteDiffs(IndentedTextWriter writer);

        //public Diff Without(PropertyInfo propertyInfo)
        //{
        //    if (this.Diffs.OfType<PropertyDiff>().Any(x => x.PropertyInfo == propertyInfo))
        //    {
        //        return new Diff(this.Diffs.Where(x => (x as PropertyDiff)?.PropertyInfo != propertyInfo).ToArray());
        //    }

        //    return this;
        //}

        //public Diff With(PropertyInfo propertyInfo, object xValue, object yValue)
        //{
        //    if (this.Diffs.OfType<PropertyDiff>().Any(x => x.PropertyInfo == propertyInfo))
        //    {
        //        var diffs = this.Diffs.Where(x => (x as PropertyDiff)?.PropertyInfo != propertyInfo).Append(new PropertyDiff(propertyInfo, xValue, yValue)).ToArray();
        //        return new Diff(diffs);
        //    }

        //    return new Diff(this.Diffs.Append(new PropertyDiff(propertyInfo, xValue, yValue)).ToArray());
        //}
    }
}
