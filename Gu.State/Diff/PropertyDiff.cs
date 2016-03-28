namespace Gu.State
{
    using System;
    using System.Reflection;

    public class PropertyDiff : Diff
    {
        private readonly ValueDiff valueDiff;

        public PropertyDiff(PropertyInfo propertyInfo, object xValue, object yValue, Diff diff)
            : base(diff.Diffs)
        {
            this.PropertyInfo = propertyInfo;
            this.valueDiff = new ValueDiff(xValue, yValue);
        }

        public PropertyDiff(PropertyInfo propertyInfo, object xValue, object yValue)
            : base(EmptyDiffs)
        {
            this.PropertyInfo = propertyInfo;
            this.valueDiff = new ValueDiff(xValue, yValue);
        }

        public PropertyInfo PropertyInfo { get; }

        public object X => this.valueDiff.X;

        public object Y => this.valueDiff.Y;

        public override string ToString()
        {
            if (this.Diffs.Count == 0)
            {
                return $"{this.PropertyInfo.Name} {this.valueDiff}";
            }

            throw new NotImplementedException("message");
        }
    }
}