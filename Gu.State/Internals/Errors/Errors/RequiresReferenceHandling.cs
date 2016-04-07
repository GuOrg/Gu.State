namespace Gu.State
{
    using System.Collections;

    internal sealed class RequiresReferenceHandling : Error, IFixWithImmutable
    {
        public static readonly RequiresReferenceHandling Enumerable = new RequiresReferenceHandling(typeof(IEnumerable).Name);
        public static readonly RequiresReferenceHandling ComplexType = new RequiresReferenceHandling("ComplexType");

        private readonly string type;

        private RequiresReferenceHandling(string type)
        {
            this.type = type;
        }

        public static bool operator ==(RequiresReferenceHandling left, RequiresReferenceHandling right)
        {
            return ReferenceEquals(left, right);
        }

        public static bool operator !=(RequiresReferenceHandling left, RequiresReferenceHandling right)
        {
            return !ReferenceEquals(left, right);
        }

        private bool Equals(RequiresReferenceHandling other)
        {
            return ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return this.type.GetHashCode();
        }
    }
}