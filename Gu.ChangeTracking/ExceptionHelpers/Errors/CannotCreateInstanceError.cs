namespace Gu.ChangeTracking
{
    using System;
    using System.Reflection;
    using System.Text;

    internal sealed class CannotCreateInstanceError : Error, INotSupported, IExcludableType, IExcludableMember
    {
        private readonly object sourceValue;

        private readonly MemberInfo member;

        public CannotCreateInstanceError(object sourceValue, MemberInfo member)
        {
            this.sourceValue = sourceValue;
            this.member = member;
        }

        public Type Type => this.sourceValue.GetType();

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            errorBuilder.AppendLine($"{typeof(Activator).Name}.{nameof(Activator.CreateInstance)} failed for type {this.sourceValue.GetType().PrettyName()}.");
            return errorBuilder;
        }

        public StringBuilder AppendSuggestExcludeMember(StringBuilder errorBuilder)
        {
            return MemberError.AppendSuggestExcludeMember(errorBuilder, this.member.DeclaringType, this.member);
        }
    }
}