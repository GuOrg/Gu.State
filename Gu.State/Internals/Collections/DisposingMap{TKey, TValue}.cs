namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal sealed class DisposingMap<TValue> : IDisposable
        where TValue : class, IDisposable
    {
        private readonly Lazy<ConcurrentDictionary<PropertyInfo, TValue>> propertyItems = new Lazy<ConcurrentDictionary<PropertyInfo, TValue>>(() => new ConcurrentDictionary<PropertyInfo, TValue>());
        private readonly Lazy<DisposingList<TValue>> collectionItems = new Lazy<DisposingList<TValue>>(() => new DisposingList<TValue>());
        private bool disposed;

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (this.collectionItems.IsValueCreated)
            {
                this.collectionItems.Value.Dispose();
            }

            if (this.propertyItems.IsValueCreated)
            {
                var keys = this.propertyItems.Value.Keys.ToList();
                foreach (var key in keys)
                {
                    TValue item;
                    if (this.propertyItems.Value.TryRemove(key, out item))
                    {
                        item?.Dispose();
                    }
                }
            }
        }

        internal void SetValue(PropertyInfo key, TValue value)
        {
            this.VerifyDisposed();
            this.propertyItems.Value.AddOrUpdate(
                key,
                k => value,
                (k, v) =>
                {
                    v?.Dispose();
                    return value;
                });
        }

        internal void SetValue(int index, TValue value)
        {
            this.VerifyDisposed();
            this.collectionItems.Value[index] = value;
        }

        internal void Remove(int index)
        {
            this.collectionItems.Value.RemoveAt(index);
        }

        internal void ClearIndexTrackers()
        {
            if (!this.collectionItems.IsValueCreated)
            {
                return;
            }

            this.collectionItems.Value.Clear();
        }

        internal void Move(int fromIndex, int toIndex)
        {
            this.collectionItems.Value.Move(fromIndex, toIndex);
        }

        internal void Reset(IReadOnlyList<TValue> newItems)
        {
            this.collectionItems.Value.Clear();
            for (var i = 0; i < newItems.Count; i++)
            {
                this.collectionItems.Value[i] = newItems[i];
            }
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