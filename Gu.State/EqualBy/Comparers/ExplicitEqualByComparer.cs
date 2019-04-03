namespace Gu.State
{
    internal class ExplicitEqualByComparer : EqualByComparer
    {
        private readonly CastingComparer comparer;

        public ExplicitEqualByComparer(CastingComparer comparer)
        {
            this.comparer = comparer;
        }

        internal override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
        {
            return TryGetEitherNullEquals(x, y, out var result)
                ? result
                : this.comparer.Equals(x, y);
        }
    }
}
