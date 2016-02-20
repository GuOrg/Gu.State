namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal sealed class ItemCollection<T> : IEnumerable<T>, IDisposable
        where T : class, IDisposable
    {
        private readonly List<T> items = new List<T>();

        internal int Count => this.items.Count;

        internal T this[int index]
        {
            get { return this.items[index]; }
            set
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                this.SetItem(index, value);
            }
        }

        public IEnumerator<T> GetEnumerator() => this.items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Dispose()
        {
            this.Clear();
        }

        internal void RemoveAt(int index)
        {
            this.TryGet(index)?.Dispose();
            this.items.RemoveAt(index);
        }

        internal void Insert(int index, T item)
        {
            this.FillTo(index);
            this.items.Insert(index, item);
        }

        internal void Move(int index, int toIndex)
        {
            var synchronizer = this.items[index];
            this.items.RemoveAt(index);
            this.Insert(toIndex, synchronizer);
        }

        internal void Clear()
        {
            for (int i = this.items.Count - 1; i >= 0; i--)
            {
                this.items[i]?.Dispose();
                this.items.RemoveAt(i);
            }
        }

        private void SetItem(int index, T item)
        {
            this.FillTo(index);
            this.TryGet(index)?.Dispose();
            this.items[index] = item;
        }

        private void FillTo(int index)
        {
            while (this.items.Count <= index)
            {
                this.items.Add(null);
            }
        }

        private T TryGet(int index)
        {
            if (index >= this.items.Count)
            {
                return null;
            }

            return this.items[index];
        }
    }
}
