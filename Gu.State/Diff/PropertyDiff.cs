namespace Gu.State
{
    using System.Reflection;

    public class PropertyDiff : Diff
    {
        private readonly ValueDiff valueDiff;

        public PropertyDiff(PropertyInfo propertyInfo, object xValue, object yValue)
            : this(propertyInfo, new ValueDiff(xValue, yValue))
        {
        }

        public PropertyDiff(PropertyInfo propertyInfo, ValueDiff diff)
            : base(diff.Diffs)
        {
            this.PropertyInfo = propertyInfo;
            this.valueDiff = diff;
        }

        public PropertyInfo PropertyInfo { get; }

        public object X => this.valueDiff.X;

        public object Y => this.valueDiff.Y;

        public override string ToString()
        {
            return $"{this.PropertyInfo.Name} {this.valueDiff}";
        }
    }
}