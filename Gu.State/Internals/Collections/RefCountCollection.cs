namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;

    internal sealed class RefCountCollection<TValue> : IDisposable
        where TValue : IDisposable
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
                trackerReference.RefCount = 0;
                trackerReference.Tracker.Dispose();
            }

            this.items.Clear();
        }

        //public bool TryAdd(TKey key, TValue value)
        //{
        //    var reference = new Reference(key, value, this);
        //    return this.items.TryAdd(key, reference);
        //}

        internal IRefCounted GetOrAdd<TKey>(TKey key, Func<TValue> creator)
            where TKey : class
        {
            this.VerifyDisposed();
            var reference = this.items.AddOrUpdate(
                key,
                k => new RefCounted(k, creator(), this),
                this.Update);
            return reference;
        }

        private void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private RefCounted Update(object key, RefCounted value)
        {
            value.RefCount++;
            return value;
        }

        private sealed class RefCounted : IRefCounted
        {
            private readonly RefCountCollection<TValue> parent;

            public RefCounted(object source, TValue tracker, RefCountCollection<TValue> parent)
            {
                this.Source = source;
                this.Tracker = tracker;
                this.parent = parent;
                this.RefCount = 1;
            }

            public object Source { get; }

            public TValue Tracker { get; }

            internal int RefCount { get; set; }

            public void Dispose()
            {
                this.RefCount--;
                if (this.RefCount == 0)
                {
                    this.Tracker.Dispose();
                    RefCounted temp;
                    this.parent.items.TryRemove(this.Source, out temp);
                }
            }
        }
    }
}