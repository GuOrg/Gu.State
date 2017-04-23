namespace Gu.State
{
    using System;
    using System.Reflection;

    internal static class MemberInfoExt
    {
        internal static Type MemberType(this MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo fieldInfo)
            {
                return fieldInfo.FieldType;
            }

            if (memberInfo is PropertyInfo propertyInfo)
            {
                return propertyInfo.PropertyType;
            }

            Throw.ExpectedParameterOfTypes<FieldInfo, PropertyInfo>(nameof(memberInfo));
            throw new InvalidOperationException("Never getting here");
        }
    }
}