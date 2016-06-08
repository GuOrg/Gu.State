namespace Gu.State
{
    using System.Collections.Generic;
    using System.Linq;

    internal class ImmutableSet<T>
    {
        internal static readonly ImmutableSet<T> Empty = new ImmutableSet<T>(new T[0]);
        private readonly HashSet<T> set;

        private ImmutableSet(IEnumerable<T> items)
        {
            this.set = new HashSet<T>(items);
        }

        internal static ImmutableSet<T> Create(IEnumerable<T> items)
        {
            if (items == null || !items.Any())
            {
                return Empty;
            }

            return new ImmutableSet<T>(items);
        }

        internal bool Contains(T value) => this.set.Contains(value);
    }
}
