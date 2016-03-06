namespace Gu.ChangeTracking
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    internal sealed class CollectionError : TypeError, IErrors, IFixWithEquatable, IFixWithImmutable, IExcludableType, IExcludableMember, INotSupported
    {
        private readonly Error error;

        public CollectionError(MemberPath memberPath, Error error)
            : base(memberPath.LastNodeType)
        {
            this.error = error;
        }

        public static bool operator ==(CollectionError left, CollectionError right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CollectionError left, CollectionError right)
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

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

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

            return obj is CollectionError && this.Equals((CollectionError)obj);
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            // nop
            return errorBuilder;
        }

        public StringBuilder AppendSuggestExcludeMember(StringBuilder errorBuilder)
        {
            // nop
            return errorBuilder;
        }

        private bool Equals(CollectionError other)
        {
            return this.Type == other.Type;
        }
    }
}