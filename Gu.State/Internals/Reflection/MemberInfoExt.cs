namespace Gu.State
{
    using System;
    using System.Reflection;

    internal static class MemberInfoExt
    {
        internal static Type MemberType(this MemberInfo memberInfo)
        {
            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return fieldInfo.FieldType;
            }

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return propertyInfo.PropertyType;
            }

            Throw.ExpectedParameterOfTypes<FieldInfo, PropertyInfo>(nameof(memberInfo));
            throw new InvalidOperationException("Never getting here");
        }
    }
}