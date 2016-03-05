namespace Gu.ChangeTracking
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal abstract class MemberError : Error, IFixWithEquatable, IExcludable
    {
        public MemberError(MemberInfo member, MemberPath path)
        {
            this.Path = path;
            this.MemberInfo = member;
        }

        public MemberInfo MemberInfo { get; }

        public MemberPath Path { get; }

        Type IFixWithEquatable.Type => this.SourceType();

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

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            var fieldInfo = this.MemberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return errorBuilder.AppendLine($"The field {this.SourceType()?.PrettyName()}.{fieldInfo.Name} of type {fieldInfo.FieldType.PrettyName()} is not supported.");
            }

            var propertyInfo = this.MemberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return errorBuilder.AppendLine($"The ¨property {this.SourceType()?.PrettyName()}.{propertyInfo.Name} of type {propertyInfo.PropertyType.PrettyName()} is not supported.");
            }

            throw Throw.ExpectedParameterOfTypes<FieldInfo, PropertyInfo>(nameof(this.MemberInfo));
        }

        public StringBuilder AppendSuggestExclude(StringBuilder errorBuilder)
        {
            var fieldInfo = this.MemberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return errorBuilder.AppendLine($"    - Exclude the field {this.SourceType()?.PrettyName()}.{fieldInfo.Name}.");
            }

            var propertyInfo = this.MemberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return errorBuilder.AppendLine($"    - Exclude the property {this.SourceType()?.PrettyName()}.{propertyInfo.Name}.");
            }

            throw Throw.ExpectedParameterOfTypes<FieldInfo, PropertyInfo>(nameof(this.MemberInfo));
        }
    }
}