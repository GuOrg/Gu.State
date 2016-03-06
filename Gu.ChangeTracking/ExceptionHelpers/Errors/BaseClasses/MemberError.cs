namespace Gu.ChangeTracking
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal abstract class MemberError : Error
    {
        protected MemberError(MemberInfo member, MemberPath path)
        {
            this.Path = path;
            this.MemberInfo = member;
        }

        public MemberInfo MemberInfo { get; }

        public MemberPath Path { get; }

        public Type SourceType()
        {
            var last = this.Path.OfType<IMemberItem>()
                                    .LastOrDefault();
            if (last != null)
            {
                return last.Member.DeclaringType;
            }

            return this.Path.Root.Type;
        }

        internal static StringBuilder AppendSuggestExcludeMember(StringBuilder errorBuilder, Type sourceType, MemberInfo member)
        {
            var fieldInfo = member as FieldInfo;
            if (fieldInfo != null)
            {
                return errorBuilder.AppendLine($"    - The field {sourceType.PrettyName()}.{fieldInfo.Name}.");
            }

            var propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                return errorBuilder.AppendLine($"    - The property {sourceType.PrettyName()}.{propertyInfo.Name}.");
            }

            throw Throw.ExpectedParameterOfTypes<FieldInfo, PropertyInfo>(nameof(member));
        }
    }
}