namespace Gu.State
{
    using System.Collections.Generic;

    internal class CastingComparer<T> : CastingComparer
    {
        private readonly IEqualityComparer<T> comparer;

        public CastingComparer(IEqualityComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        public override bool Equals(object x, object y)
        {
            return this.comparer.Equals((T)x, (T)y);
        }

        public override int GetHashCode(object obj)
        {
            return this.comparer.GetHashCode((T)obj);
        }
    }
}
