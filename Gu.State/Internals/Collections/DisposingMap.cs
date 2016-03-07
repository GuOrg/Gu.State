namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class DisposingMap<TKey, TValue> : IDisposable
        where TValue : IDisposable
    {
        private readonly ConcurrentDictionary<TKey, TValue> items = new ConcurrentDictionary<TKey, TValue>();
        private bool disposed;

        internal IEnumerable<TValue> Values => this.items.Values;
         
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            while (this.items.Count > 0)
            {
                TValue item;
                if (this.items.TryRemove(this.items.Keys.First(), out item))
                {
                    item?.Dispose();
                }
            }
        }

        internal void SetValue(TKey key, TValue value)
        {
            this.VerifyDisposed();
            this.items.AddOrUpdate(key, value, new Updater(value).UpdateValue);
        }

        private void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private struct Updater
        {
            private readonly TValue newValue;

            internal Updater(TValue newValue)
            {
                this.newValue = newValue;
            }

            internal TValue UpdateValue(TKey _, TValue oldValue)
            {
                oldValue?.Dispose();
                return this.newValue;
            }
        }
    }
}