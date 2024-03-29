namespace Gu.State
{
    internal sealed class RequiresReferenceHandling : Error, IFixWithImmutable
    {
        internal static readonly RequiresReferenceHandling Default = new();

        private RequiresReferenceHandling()
        {
        }

        public static bool operator ==(RequiresReferenceHandling left, RequiresReferenceHandling right)
        {
            return ReferenceEquals(left, right);
        }

        public static bool operator !=(RequiresReferenceHandling left, RequiresReferenceHandling right)
        {
            return !ReferenceEquals(left, right);
        }

        public override bool Equals(object obj) => ReferenceEquals(this, obj);

        public override int GetHashCode() => 0;
    }
}
