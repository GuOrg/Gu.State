namespace Gu.State
{
    using System.Reflection;

    /// <summary>A diff for a property.</summary>
    public class PropertyDiff : MemberDiff<PropertyInfo>
    {
        /// <summary> Initializes a new instance of the <see cref="PropertyDiff"/> class.</summary>
        /// <param name="propertyInfo">The property.</param>
        /// <param name="xValue">The x value of the <paramref name="propertyInfo"/>.</param>
        /// <param name="yValue">The y value of the <paramref name="propertyInfo"/>.</param>
        public PropertyDiff(PropertyInfo propertyInfo, object xValue, object yValue)
            : this(propertyInfo, new ValueDiff(xValue, yValue))
        {
        }

        /// <summary> Initializes a new instance of the <see cref="PropertyDiff"/> class.</summary>
        /// <param name="propertyInfo">The property.</param>
        /// <param name="diff">The <see cref="ValueDiff"/> for the <paramref name="propertyInfo"/>.</param>
        public PropertyDiff(PropertyInfo propertyInfo, ValueDiff diff)
            : base(propertyInfo, diff)
        {
        }

        /// <summary>Gets the property.</summary>
        public PropertyInfo PropertyInfo => this.MemberInfo;
    }
}