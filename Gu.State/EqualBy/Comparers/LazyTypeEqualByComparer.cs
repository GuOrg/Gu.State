namespace Gu.State
{
    internal class LazyTypeEqualByComparer : EqualByComparer
    {
        internal static readonly LazyTypeEqualByComparer Default = new LazyTypeEqualByComparer();

        private LazyTypeEqualByComparer()
        {
        }

        public override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
        {
            return TryGetEitherNullEquals(x, y, out var result)
                ? result
                : settings.GetEqualByComparer(x.GetType(), checkReferenceHandling: true).Equals(x, y, settings, referencePairs);
        }
    }
}