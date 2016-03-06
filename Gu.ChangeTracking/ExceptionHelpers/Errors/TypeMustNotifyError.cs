namespace Gu.ChangeTracking
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    internal sealed class TypeMustNotifyError : TypeError, IErrors, IFixWithImmutable, IExcludableType, IExcludableMember, INotSupported
    {
        private readonly MemberPath path;
        private readonly Error error;

        public TypeMustNotifyError(MemberPath memberPath, Error error)
            : base(memberPath.LastNodeType)
        {
            this.path = memberPath;
            this.error = error;
        }

        public static bool operator ==(TypeMustNotifyError left, TypeMustNotifyError right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TypeMustNotifyError left, TypeMustNotifyError right)
        {
            return !Equals(left, right);
        }

        public IEnumerator<Error> GetEnumerator()
        {
            yield return this;
            var errors = this.error as IErrors;
            if (errors != null)
            {
                foreach (var e in errors)
                {
                    yield return e;
                }
            }
            else
            {
                yield return this.error;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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