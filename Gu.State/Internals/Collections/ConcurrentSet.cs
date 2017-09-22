namespace Gu.State
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    internal class ConcurrentSet<T> : ICollection<T>
    {
        private readonly ConcurrentDictionary<T, int> inner;

        public ConcurrentSet()
        {
            this.inner = new ConcurrentDictionary<T, int>();
        }

        public ConcurrentSet(IEqualityComparer<T> comparer)
        {
            this.inner = new ConcurrentDictionary<T, int>(comparer);
        }

        public int Count => this.inner.Count;

        bool ICollection<T>.IsReadOnly => false;

        public bool Add(T item) => this.inner.TryAdd(item, 0);

        public void Clear() => this.inner.Clear();

        void ICollection<T>.Add(T item) => this.Add(item);

        public bool Contains(T item) => this.inner.ContainsKey(item);

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => this.inner.Keys.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            return this.inner.TryRemove(item, out var temp);
        }

        public IEnumerator<T> GetEnumerator() => this.inner.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}