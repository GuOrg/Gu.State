namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal sealed class RefCountCollection<TValue> : IDisposable
    {
        private readonly ConcurrentDictionary<object, RefCounted> items = new ConcurrentDictionary<object, RefCounted>(ReferenceComparer.Default);
        private bool disposed;

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            foreach (var trackerReference in this.items.Values)
            {
                (trackerReference.Tracker as IDisposable)?.Dispose();
            }

            this.items.Clear();
        }

        //public bool TryAdd(TKey key, TValue value)
        //{
        //    var reference = new Reference(key, value, this);
        //    return this.items.TryAdd(key, reference);
        //}

        internal IRefCounted<TValue> GetOrAdd<TOwner, TKey>(TOwner owner, TKey key, Func<TValue> creator)
            where TKey : class
        {
            this.VerifyDisposed();
            var reference = this.items.AddOrUpdate(
                key,
                k => new RefCounted(k, creator(), this),
                (k, v) => this.Update(owner, k, v));
            return reference;
        }

        private void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private RefCounted Update(object owner, object key, RefCounted value)
        {
            value.AddOwner(owner);
            return value;
        }

        private sealed class RefCounted : IRefCounted<TValue>
        {
            private readonly RefCountCollection<TValue> parent;
            private readonly List<object> owners = new List<object>();

            public RefCounted(object source, TValue tracker, RefCountCollection<TValue> parent)
            {
                this.Source = source;
                this.Tracker = tracker;
                this.parent = parent;
            }

            public object Source { get; }

            public TValue Tracker { get; }

            public void Dispose()
            {
                lock (this.owners)
                {
                    Debug.Assert(this.owners.Count == 0, "Cannot dispose if owners is not null");
                    this.owners.Clear();
                    (this.Tracker as IDisposable)?.Dispose();
                    RefCounted temp;
                    this.parent.items.TryRemove(this.Source, out temp);
                }
            }

            public CanDispose RemoveOwner<TOwner>(TOwner owner)
                    where TOwner : class
            {
                lock (this.owners)
                {
                    if (!this.owners.Remove(owner))
                    {
                        return CanDispose.No;
                    }

                    if (this.owners.Count == 0)
                    {
                        this.Dispose();
                        return CanDispose.Yes;
                    }

                    return CanDispose.No;
                }
            }

            internal void AddOwner(object owner)
            {
                lock (this.owners)
                {
                    this.owners.Add(owner);
                }
            }
        }
    }
}