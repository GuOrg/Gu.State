namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal sealed class DisposingList<T> : IDisposable
        where T : class, IDisposable
    {
        private readonly List<T> items;
        private bool disposed;

        public DisposingList()
        {
            this.items = new List<T>();
        }

        public DisposingList(int count)
        {
            this.items = new List<T>(count);
        }

        internal int Count => this.items.Count;

        internal IReadOnlyList<T> Items => this.items;

        internal T this[int index]
        {
            get
            {
                lock (this.items)
                {
                    return this.items[index];
                }
            }

            set
            {
                this.VerifyDisposed();
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                lock (this.items)
                {
                    this.VerifyDisposed();
                    this.SetItem(index, value);
                }
            }
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.Clear();
        }

        internal void RemoveAt(int index)
        {
            lock (this.items)
            {
                this.VerifyDisposed();
                this.TryGet(index)?.Dispose();
                this.items.RemoveAt(index);
            }
        }

        internal void Insert(int index, T item)
        {
            lock (this.items)
            {
                this.VerifyDisposed();
                this.FillTo(index);
                this.items.Insert(index, item);
            }
        }

        internal void Move(int index, int toIndex)
        {
            lock (this.items)
            {
                this.VerifyDisposed();
                var synchronizer = this.items[index];
                this.items.RemoveAt(index);
                this.Insert(toIndex, synchronizer);
            }
        }

        internal void Clear()
        {
            lock (this.items)
            {
                for (int i = this.items.Count - 1; i >= 0; i--)
                {
                    this.items[i]?.Dispose();
                    this.items.RemoveAt(i);
                }
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

        private void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}
