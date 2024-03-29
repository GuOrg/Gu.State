namespace Gu.State
{
    using System.Collections;
    using System.Collections.Generic;

    internal class ExplicitEqualByComparer<T> : EqualByComparer<T>
    {
        internal static ExplicitEqualByComparer<T> Default = new(EqualityComparer<T>.Default);

        internal readonly IEqualityComparer<T> EqualityComparer;

        internal ExplicitEqualByComparer(IEqualityComparer<T> equalityComparer)
        {
            this.EqualityComparer = equalityComparer;
        }

        internal override bool CanHaveReferenceLoops => false;

        internal static ExplicitEqualByComparer<T> Create(IEqualityComparer comparer) => new((IEqualityComparer<T>)comparer);

        internal override bool TryGetError(MemberSettings settings, out Error error)
        {
            error = null;
            return false;
        }

        internal override bool Equals(T x, T y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
        {
            return this.EqualityComparer.Equals(x, y);
        }

        internal override bool Equals(object x, object y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
        {
            return TryGetEitherNullEquals(x, y, out var result)
                ? result
                : this.Equals((T)x, (T)y, settings, referencePairs);
        }
    }
}
