namespace Gu.State
{
    using System.Reflection;

    public class PropertyDiff : MemberDiff<PropertyInfo>
    {
        public PropertyDiff(PropertyInfo propertyInfo, object xValue, object yValue)
            : this(propertyInfo, new ValueDiff(xValue, yValue))
        {
        }

        public PropertyDiff(PropertyInfo propertyInfo, ValueDiff diff)
            : base(propertyInfo, diff)
        {
        }

        public PropertyInfo PropertyInfo => this.MemberyInfo;
    }
}