namespace Gu.State
{
    using System;
    using System.Reflection;
    using System.Text;

    internal static partial class StringBuilderExt
    {
        internal static StringBuilder AppendSolveTheProblemBy(this StringBuilder errorBuilder)
        {
            return errorBuilder.AppendLine("Solve the problem by any of:");
        }

        internal static StringBuilder AppendSuggestExcludeMember(this StringBuilder errorBuilder, MemberInfo member)
        {
            var fieldInfo = member as FieldInfo;
            if (fieldInfo != null)
            {
                return errorBuilder.AppendLine($"    - The field {member.DeclaringType.PrettyName()}.{fieldInfo.Name}.");
            }

            var propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                if (propertyInfo.GetIndexParameters().Length > 0)
                {
                    return errorBuilder.AppendLine($"    - The indexer property {propertyInfo.DeclaringType.PrettyName()}.{propertyInfo.Name}.");
                }

                return errorBuilder.AppendLine($"    - The property {member.DeclaringType.PrettyName()}.{propertyInfo.Name}.");
            }

            throw Throw.ExpectedParameterOfTypes<FieldInfo, PropertyInfo>(nameof(member));
        }

        internal static StringBuilder AppendNotSupportedMember(this StringBuilder errorBuilder, MemberInfo member)
        {
            var fieldInfo = member as FieldInfo;
            if (fieldInfo != null)
            {
                return errorBuilder.AppendLine($"The field {fieldInfo.DeclaringType.PrettyName()}.{fieldInfo.Name} of type {fieldInfo.FieldType.PrettyName()} is not supported.");
            }

            var propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                if (propertyInfo.GetIndexParameters().Length > 0)
                {
                    return errorBuilder.AppendLine($"  - The property {propertyInfo.DeclaringType.PrettyName()}.{propertyInfo.Name} is an indexer and not supported.");
                }

                return errorBuilder.AppendLine($"The property {propertyInfo.DeclaringType.PrettyName()}.{propertyInfo.Name} of type {propertyInfo.PropertyType.PrettyName()} is not supported.");
            }

            throw Throw.ExpectedParameterOfTypes<FieldInfo, PropertyInfo>(nameof(member));
        }
    }
}
