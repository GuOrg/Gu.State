namespace Gu.State
{
    using System.Collections;

    internal sealed class RequiresReferenceHandling : Error, IFixWithImmutable
    {
        private readonly string type;

        public static readonly RequiresReferenceHandling Enumerable = new RequiresReferenceHandling(typeof(IEnumerable).Name);

        public static readonly RequiresReferenceHandling Other = new RequiresReferenceHandling("Other");

        private RequiresReferenceHandling(string type)
        {
            this.type = type;
        }

        public static bool operator ==(RequiresReferenceHandling left, RequiresReferenceHandling right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RequiresReferenceHandling left, RequiresReferenceHandling right)
        {
            return !Equals(left, right);
        }

        private bool Equals(RequiresReferenceHandling other)
        {
            return string.Equals(this.type, other.type);
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

            return obj is RequiresReferenceHandling && this.Equals((RequiresReferenceHandling)obj);
        }

        public override int GetHashCode()
        {
            return this.type.GetHashCode();
        }
    }
}