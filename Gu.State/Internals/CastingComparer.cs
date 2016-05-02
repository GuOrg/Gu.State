namespace Gu.State
{
    using System.Collections.Generic;

    public abstract class CastingComparer
    {
        public static CastingComparer Create<T>(IEqualityComparer<T> comparer)
        {
            return new CastingComparer<T>(comparer);
        }

        public new abstract bool Equals(object x, object y);

        public abstract int GetHashCode(object obj);
    }
}