namespace Gu.State
{
    using System.Collections.Generic;

    internal class ImmutableSet<T>
    {
        internal static readonly ImmutableSet<T> Empty = new ImmutableSet<T>(new T[0]);
        private readonly HashSet<T> set;

        internal ImmutableSet(IEnumerable<T> items)
        {
            this.set = new HashSet<T>(items);
        }

        internal bool Contains(T value) => this.set.Contains(value);
    }
}
