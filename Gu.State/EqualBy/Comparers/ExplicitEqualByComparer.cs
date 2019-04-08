namespace Gu.State
{
    using System.Collections;
    using System.Collections.Generic;

    internal class ExplicitEqualByComparer<T> : EqualByComparer<T>
    {
        private readonly IEqualityComparer<T> comparer;

        public ExplicitEqualByComparer(IEqualityComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        internal static ExplicitEqualByComparer<T> Create(IEqualityComparer comparer) => new ExplicitEqualByComparer<T>((IEqualityComparer<T>)comparer);

        internal override bool TryGetError(MemberSettings settings, out Error error)
        {
            error = null;
            return false;
        }

        internal override bool Equals(T x, T y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
        {
            return this.comparer.Equals(x, y);
        }

        internal override bool Equals(object x, object y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
        {
            return TryGetEitherNullEquals(x, y, out var result)
                ? result
                : this.Equals((T)x, (T)y, settings, referencePairs);
        }
    }
}
