namespace Gu.State
{
    using System.Collections;
    using System.Collections.Generic;

    internal class ImmutableArray<T> : IReadOnlyList<T>
    {
        private readonly T[] items;

        internal ImmutableArray(T[] items)
        {
            this.items = items;
        }

        public int Count => ((IReadOnlyList<T>)this.items).Count;

        public T this[int index] => ((IReadOnlyList<T>)this.items)[index];

        public IEnumerator<T> GetEnumerator() => ((IReadOnlyList<T>)this.items).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IReadOnlyList<T>)this.items).GetEnumerator();
    }
}
