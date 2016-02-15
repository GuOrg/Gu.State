namespace Gu.ChangeTracking
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public class SpecialCopyProperty : ISpecialCopyProperty
    {
        public SpecialCopyProperty(PropertyInfo property, Func<object, object> createCopyValue)
        {
            this.Property = property;
            this.CreateCopyValue = createCopyValue;
        }

        public PropertyInfo Property { get; }

        public Func<object, object> CreateCopyValue { get; }

        public static SpecialCopyProperty Create<TSource, TPropertyValue>(
            Expression<Func<TSource, TPropertyValue>> property,
            Func<TPropertyValue, TPropertyValue> createCopyValue)
        {
            var memberExpression = (MemberExpression)property.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            return new SpecialCopyProperty(propertyInfo, source => createCopyValue((TPropertyValue)source));
        }
    }
}