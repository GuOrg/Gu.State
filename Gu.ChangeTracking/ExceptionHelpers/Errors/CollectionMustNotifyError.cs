namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text;

    internal sealed class CollectionMustNotifyError : TypeError, IErrors, IFixWithImmutable, IExcludableType, IExcludableMember, INotSupported
    {
        private readonly MemberPath path;
        private readonly Error error;

        public CollectionMustNotifyError(MemberPath memberPath, Error error)
            : base(memberPath.LastNodeType)
        {
            this.path = memberPath;
            this.error = error;
        }

        Type IFixWithImmutable.Type => this.path.LastMember.MemberType();

        public static bool operator ==(CollectionMustNotifyError left, CollectionMustNotifyError right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CollectionMustNotifyError left, CollectionMustNotifyError right)
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
            var line = this.Type.Assembly == typeof(List<>).Assembly
                           ? $"The collection type {this.Type.PrettyName()} does not notify changes. Use a type that implements {typeof(INotifyCollectionChanged).Name}."
                           : $"The collection type {this.Type.PrettyName()} does not notify changes. Implement {typeof(INotifyCollectionChanged).Name} or use a type that implements {typeof(INotifyCollectionChanged).Name}.";
            return errorBuilder.AppendLine(line);
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

            return obj is CollectionMustNotifyError && this.Equals((CollectionMustNotifyError)obj);
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }

        private bool Equals(CollectionMustNotifyError other)
        {
            return this.Type == other.Type;
        }
    }
}