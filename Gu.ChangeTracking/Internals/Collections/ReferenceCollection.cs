namespace Gu.ChangeTracking
{
    using System.Collections.Generic;

    internal class ReferenceCollection
    {
        private readonly HashSet<object> items = new HashSet<object>(ReferenceComparer.Default);

        internal void Add(object x)
        {
            if (x == null)
            {
                return;
            }

            var type = x.GetType();
            if (type.IsValueType || type.IsEnum)
            {
                return;
            }

            this.items.Add(x);
        }

        internal bool Contains(object x)
        {
            return this.items.Contains(x);
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