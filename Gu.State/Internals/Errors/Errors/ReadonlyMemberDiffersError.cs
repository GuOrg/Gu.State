namespace Gu.State
{
    using System;
    using System.Reflection;
    using System.Text;

    internal sealed class ReadonlyMemberDiffersError : Error, INotSupported, IExcludableMember
    {
        internal ReadonlyMemberDiffersError(SourceAndTargetValue sourceAndTargetValue, MemberInfo member)
        {
            this.SourceAndTargetValue = sourceAndTargetValue;
            this.Member = member;
        }

        public MemberInfo Member { get; }

        internal SourceAndTargetValue SourceAndTargetValue { get; }

        internal Type Type => this.SourceAndTargetValue.Source.GetType();

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
                    throw new InvalidOperationException($"Unhandled member type {this.Member.MemberType}");
            }

            errorBuilder.AppendLine($" - Source value {this.GetType(this.SourceAndTargetValue.SourceValue)}: {this.GetValue(this.SourceAndTargetValue.SourceValue)}.")
                        .AppendLine($" - Target value {this.GetType(this.SourceAndTargetValue.TargetValue)}: {this.GetValue(this.SourceAndTargetValue.TargetValue)}.");
            return errorBuilder;
        }

        private string GetType(object value)
        {
            if (value is null)
            {
                return string.Empty;
            }

            return $"({value.GetType().PrettyName()})";
        }

        private string GetValue(object value)
        {
            if (value is null)
            {
                return "null";
            }

            return value.ToString();
        }
    }
}
