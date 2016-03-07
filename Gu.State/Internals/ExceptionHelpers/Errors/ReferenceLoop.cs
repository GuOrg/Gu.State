namespace Gu.State
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    [DebuggerDisplay("{GetType().Name} Indexer: {Path.PathString}")]
    internal sealed class ReferenceLoop : MemberError, INotSupported, IExcludableType, IFixWithEquatable, IFixWithImmutable
    {
        public ReferenceLoop(MemberInfo member, MemberPath path)
            : base(member, path)
        {
        }

        public Type Type => this.SourceType();

        Type IExcludableType.Type => this.MemberInfo.MemberType();

        public static bool operator ==(ReferenceLoop left, ReferenceLoop right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ReferenceLoop left, ReferenceLoop right)
        {
            return !Equals(left, right);
        }

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            var fieldInfo = this.MemberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return errorBuilder.AppendLine($"The field {this.Type.PrettyName()}.{fieldInfo.Name} of type {fieldInfo.FieldType.PrettyName()} is in a reference loop.")
                                   .AppendLine($"  - The loop is {this.Path.PathString}...");
            }

            var propertyInfo = this.MemberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return errorBuilder.AppendLine($"The property {this.Type.PrettyName()}.{propertyInfo.Name} of type {propertyInfo.PropertyType.PrettyName()} is in a reference loop.")
                                   .AppendLine($"  - The loop is {this.Path.PathString}...");
            }

            throw Throw.ExpectedParameterOfTypes<FieldInfo, PropertyInfo>(nameof(this.MemberInfo));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is ReferenceLoop && this.Equals((ReferenceLoop)obj);
        }

        public override int GetHashCode()
        {
            return this.GetType().GetHashCode() ^ this.MemberInfo.GetHashCode();
        }

        private bool Equals(ReferenceLoop other)
        {
            return this.MemberInfo == other.MemberInfo;
        }
    }
}