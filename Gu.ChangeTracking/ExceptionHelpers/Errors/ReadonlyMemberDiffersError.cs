namespace Gu.ChangeTracking
{
    using System;
    using System.Reflection;
    using System.Text;

    internal sealed class ReadonlyMemberDiffersError : Error, INotSupported, IExcludableMember, IExcludableType
    {
        public ReadonlyMemberDiffersError(SourceAndTargetValue sourceAndTargetValue, MemberInfo member)
        {
            this.SourceAndTargetValue = sourceAndTargetValue;
            this.Member = member;
        }

        public SourceAndTargetValue SourceAndTargetValue { get; }

        public MemberInfo Member { get; }

        public Type Type => this.SourceAndTargetValue.Source.GetType();

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            switch (this.Member.MemberType)
            {
                case MemberTypes.Field:
                    errorBuilder.AppendLine($"The readonly field {this.Type.PrettyName()}.{this.Member.Name} differs after copy.");
                    break;
                case MemberTypes.Property:
                    errorBuilder.AppendLine($"The readonly property {this.Type.PrettyName()}.{this.Member.Name} differs after copy.");
                    break;
                case MemberTypes.Constructor:
                case MemberTypes.Event:
                case MemberTypes.Method:
                case MemberTypes.TypeInfo:
                case MemberTypes.Custom:
                case MemberTypes.NestedType:
                case MemberTypes.All:
                    throw Throw.ExpectedParameterOfTypes<PropertyInfo, FieldInfo>(nameof(this.Member));
                default:
                    throw new ArgumentOutOfRangeException();
            }

            errorBuilder.AppendLine($" - Source value {this.GetType(this.SourceAndTargetValue.SourceValue)}: {this.GetValue(this.SourceAndTargetValue.SourceValue)}.")
                        .AppendLine($" - Target value {this.GetType(this.SourceAndTargetValue.TargeteValue)}: {this.GetValue(this.SourceAndTargetValue.TargeteValue)}.");
            return errorBuilder;
        }

        public StringBuilder AppendSuggestExclude(StringBuilder errorBuilder)
        {
            return MemberError.AppendSuggestExcludeMember(errorBuilder, this.Type, this.Member);
        }

        private string GetType(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return $"({value.GetType().PrettyName()})";
        }

        private string GetValue(object value)
        {
            if (value == null)
            {
                return "null";
            }

            return value.ToString();
        }
    }
}
