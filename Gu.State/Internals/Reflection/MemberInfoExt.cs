namespace Gu.State
{
    using System;
    using System.Reflection;

    internal static class MemberInfoExt
    {
        internal static Type MemberType(this MemberInfo memberInfo)
        {
            return memberInfo switch
            {
                FieldInfo fieldInfo => fieldInfo.FieldType,
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                _ => throw Throw.ShouldNeverGetHereException("Expected FieldInfo or PropertyInfo."),
            };
        }

        internal static bool IsIndexer(this MemberInfo member)
        {
            return member is PropertyInfo property &&
                   property.GetIndexParameters().Length > 0;
        }
    }
}
