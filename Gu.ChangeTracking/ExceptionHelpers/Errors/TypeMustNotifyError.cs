namespace Gu.ChangeTracking
{
    using System.ComponentModel;
    using System.Text;

    internal sealed class TypeMustNotifyError : TypeError, IFixWithImmutable, IExcludableType, IExcludableMember, INotSupported
    {
        private readonly MemberPath path;

        public TypeMustNotifyError(MemberPath memberPath)
            : base(memberPath.LastNodeType)
        {
            this.path = memberPath;
        }

        public static bool operator ==(TypeMustNotifyError left, TypeMustNotifyError right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TypeMustNotifyError left, TypeMustNotifyError right)
        {
            return !Equals(left, right);
        }

        StringBuilder IExcludableMember.AppendSuggestExcludeMember(StringBuilder errorBuilder)
        {
            var lastMember = this.path.LastMember;
            return MemberError.AppendSuggestExcludeMember(errorBuilder, lastMember.DeclaringType, lastMember);
        }

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            var line = this.Type.Assembly == typeof(int).Assembly
                           ? $"The type {this.Type.PrettyName()} does not notify changes. Use a type that implements {typeof(INotifyPropertyChanged).Name}."
                           : $"The type {this.Type.PrettyName()} does not notify changes. Implement {typeof(INotifyPropertyChanged).Name} or use a type that implements {typeof(INotifyPropertyChanged).Name}.";
            return errorBuilder.AppendLine(line);
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

            return obj is TypeMustNotifyError && this.Equals((TypeMustNotifyError)obj);
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }

        private bool Equals(TypeMustNotifyError other)
        {
            return this.Type == other.Type;
        }
    }
}