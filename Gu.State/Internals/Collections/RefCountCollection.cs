namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal sealed class RefCountCollection<TValue> : IDisposable
        where TValue : IRefCountable
    {
        private readonly ConcurrentDictionary<object, RefCounted> items = new ConcurrentDictionary<object, RefCounted>(ReferenceComparer.Default);
        private bool disposed;

        internal int Count => this.items.Count;

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            foreach (var trackerReference in this.items.Values)
            {
                trackerReference.Tracker.Dispose();
            }

            this.items.Clear();
        }

        internal IRefCounted<TValue> GetOrAdd<TOwner, TKey>(TOwner owner, TKey key, Func<TValue> creator)
            where TKey : class
        {
            this.VerifyDisposed();
            return this.GetOrAdd(owner, (object)key, creator);
        }

        internal IRefCounted<TValue> GetOrAdd<TOwner>(TOwner owner, ReferencePair key, Func<TValue> creator)
        {
            this.VerifyDisposed();
            return this.GetOrAdd(owner, (object)key, creator);
        }

        private void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private IRefCounted<TValue> GetOrAdd<TOwner>(TOwner owner, object key, Func<TValue> creator)
        {
            var reference = this.items.AddOrUpdate(
                key,
                k => new RefCounted(owner, k, creator(), this),
                (k, v) => this.Update(owner, v));
            return reference;
        }

        private RefCounted Update(object owner, RefCounted value)
        {
            value.AddOwner(owner);
            return value;
        }

        private sealed class RefCounted : IRefCounted<TValue>
        {
            private readonly RefCountCollection<TValue> parent;
            private readonly List<object> owners = new List<object>();

            public RefCounted(object owner, object source, TValue tracker, RefCountCollection<TValue> parent)
            {
                this.owners.Add(owner);
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
                    this.Tracker.Dispose();
                    RefCounted temp;
                    this.parent.items.TryRemove(this.Source, out temp);
                }
            }

            public void RemoveOwner<TOwner>(TOwner owner)
                    where TOwner : class
            {
                lock (this.owners)
                {
                    if (!this.owners.Remove(owner))
                    {
                        Debug.Assert(false, "Owners did not contain owner");
                    }

                    if (this.owners.Count == 0)
                    {
                        this.Dispose();
                    }
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