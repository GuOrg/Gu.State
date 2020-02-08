namespace Gu.State
{
    using System.Collections.Generic;

    internal class CastingComparer<T> : CastingComparer, IEqualityComparer<T>
    {
        private readonly IEqualityComparer<T> comparer;

        internal CastingComparer(IEqualityComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        public override bool Equals(object x, object y) => this.comparer.Equals((T)x, (T)y);

        public override int GetHashCode(object obj) => this.comparer.GetHashCode((T)obj);

        public bool Equals(T x, T y) => this.comparer.Equals(x, y);

        public int GetHashCode(T obj) => this.comparer.GetHashCode(obj);
    }
}
