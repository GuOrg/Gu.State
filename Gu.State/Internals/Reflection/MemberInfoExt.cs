namespace Gu.State
{
    using System;
    using System.Reflection;

    internal static class MemberInfoExt
    {
        internal static Type MemberType(this MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    return fieldInfo.FieldType;
                case PropertyInfo propertyInfo:
                    return propertyInfo.PropertyType;
                default:
                    throw Throw.ShouldNeverGetHereException("Expected FieldInfo or PropertyInfo.");
            }
        }

        internal static bool IsIndexer(this MemberInfo member)
        {
            return member is PropertyInfo property &&
                   property.GetIndexParameters().Length > 0;
        }
    }
}