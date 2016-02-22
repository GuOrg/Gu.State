namespace Gu.ChangeTracking
{
    using System.Collections.Generic;

    public class ReferenceCollection
    {
        private readonly HashSet<object> pairs = new HashSet<object>(ReferenceComparer.Default);

        public static bool IsReferenceType(object x)
        {
            if (x == null)
            {
                return false;
            }

            var type = x.GetType();
            return !EqualBy.IsEquatable(type);
        }

        public void Add(object x)
        {
            this.pairs.Add(x);
        }

        public void Remove(object x)
        {
            this.pairs.Remove(x);
        }

        public bool Contains(object x)
        {
            return this.pairs.Contains(x);
        }

        private class ReferenceComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceComparer Default = new ReferenceComparer();

            private ReferenceComparer()
            {
            }

            bool IEqualityComparer<object>.Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }

            int IEqualityComparer<object>.GetHashCode(object obj)
            {
                return obj?.GetHashCode() ?? 0;
            }
        }
    }
}