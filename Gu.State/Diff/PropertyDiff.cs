namespace Gu.State
{
    using System.Reflection;

    public class PropertyDiff : Diff
    {
        public PropertyDiff(PropertyInfo propertyInfo, object xValue, object yValue)
            : base(EmptyDiffs)
        {
            this.PropertyInfo = propertyInfo;
            this.X = xValue;
            this.Y = yValue;
        }

        public PropertyInfo PropertyInfo { get; }

        public object X { get; }

        public object Y { get; }
    }
}