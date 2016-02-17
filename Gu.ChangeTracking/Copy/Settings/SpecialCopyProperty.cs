namespace Gu.ChangeTracking
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public class SpecialCopyProperty : ISpecialCopyProperty
    {
        private readonly Action<object, object> copyValue;

        public SpecialCopyProperty(PropertyInfo property, Action<object, object> copyValue)
        {
            this.copyValue = copyValue;
            this.Property = property;
        }

        public PropertyInfo Property { get; }

        public void CopyValue(object source, object target)
        {
            this.copyValue(source, target);
        }

        public static SpecialCopyProperty CreateNop<T>(string propertyName, BindingFlags? bindingFlags)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName, bindingFlags ?? Constants.DefaultPropertyBindingFlags);
            return new SpecialCopyProperty(propertyInfo, (s, t) => { });
        }

        public static SpecialCopyProperty CreateClone<TSource, TPropertyValue>(
            Expression<Func<TSource, TPropertyValue>> property, Func<TPropertyValue> createNew)
            where TPropertyValue : class
        {
            return Create(property, source =>
            {
                var sourceValue = property.Compile().Invoke(source);
                if (sourceValue == null)
                {
                    return null;
                }

                var @new = createNew();
                Copy.PropertyValues(sourceValue, @new);
                return @new;
            });
        }

        public static SpecialCopyProperty Create<TSource, TPropertyValue>(
            Expression<Func<TSource, TPropertyValue>> property,
            Func<TSource, TPropertyValue> createCopyValue)
        {
            var memberExpression = (MemberExpression)property.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            return new SpecialCopyProperty(propertyInfo, (source, target) => propertyInfo.SetValue(target, createCopyValue((TSource)source)));
        }
    }
}