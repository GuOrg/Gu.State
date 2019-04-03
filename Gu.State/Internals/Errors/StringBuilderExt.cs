namespace Gu.State
{
    using System.Linq;
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
            if (member is FieldInfo fieldInfo)
            {
                return errorBuilder.AppendLine($"    - The field {member.DeclaringType.PrettyName()}.{fieldInfo.Name}.");
            }

            if (member is PropertyInfo propertyInfo)
            {
                if (propertyInfo.GetIndexParameters().Length > 0)
                {
                    return errorBuilder.AppendLine($"    - The indexer property {propertyInfo.DeclaringType.PrettyName()}[{string.Join(", ", propertyInfo.GetIndexParameters().Select(x => x.ParameterType.PrettyName()))}].");
                }

                return errorBuilder.AppendLine($"    - The property {member.DeclaringType.PrettyName()}.{propertyInfo.Name}.");
            }

            throw Throw.ExpectedParameterOfTypes<FieldInfo, PropertyInfo>(nameof(member));
        }

        internal static StringBuilder AppendNotSupportedMember(this StringBuilder errorBuilder, MemberInfo member)
        {
            if (member is FieldInfo fieldInfo)
            {
                return errorBuilder.AppendLine($"The field {fieldInfo.DeclaringType.PrettyName()}.{fieldInfo.Name} of type {fieldInfo.FieldType.PrettyName()} is not supported.");
            }

            if (member is PropertyInfo propertyInfo)
            {
                if (propertyInfo.GetIndexParameters().Length > 0)
                {
                    return errorBuilder.AppendLine($"  - The property {propertyInfo.DeclaringType.PrettyName()}[{string.Join(",", propertyInfo.GetIndexParameters().Select(x => x.ParameterType.PrettyName()))}] is an indexer and not supported.");
                }

                return errorBuilder.AppendLine($"The property {propertyInfo.DeclaringType.PrettyName()}.{propertyInfo.Name} of type {propertyInfo.PropertyType.PrettyName()} is not supported.");
            }

            throw Throw.ExpectedParameterOfTypes<FieldInfo, PropertyInfo>(nameof(member));
        }
    }
}
