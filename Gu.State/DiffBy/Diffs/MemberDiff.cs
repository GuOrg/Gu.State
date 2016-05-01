namespace Gu.State
{
    using System;
    using System.Reflection;

    public static class MemberDiff
    {
        public static SubDiff Create(MemberInfo member, object xValue, object yValue)
        {
            return Create(member, new ValueDiff(xValue, yValue));
        }

        internal static SubDiff Create(MemberInfo member, ValueDiff diff)
        {
            var fieldInfo = member as FieldInfo;
            if (fieldInfo != null)
            {
                return new FieldDiff(fieldInfo, diff);
            }

            var propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                return new PropertyDiff(propertyInfo, diff);
            }

            throw State.Throw.ExpectedParameterOfTypes<PropertyInfo, FieldInfo>("MemberDiff.Create(MemberInfo member, object xValue, object yValue)");
        }
    }
}