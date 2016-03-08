namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;

    internal sealed class RefCountCollection<TKey, TValue> : IDisposable
         where TValue : IDisposable
    {
        private readonly ConcurrentDictionary<TKey, Reference> items = new ConcurrentDictionary<TKey, Reference>();
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

        public bool TryAdd(TKey key, TValue tracker)
        {
            var reference = new Reference(key, tracker, this);
            return this.items.TryAdd(key, reference);
        }

        internal TValue GetOrAdd(TKey key, Func<TValue> creator)
        {
            this.VerifyDisposed();
            var reference = this.items.AddOrUpdate(
                key,
                k => new Reference(k, creator(), this),
                this.Update);
            return reference.Tracker;
        }

        private void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private Reference Update(TKey key, Reference value)
        {
            value.RefCount++;
            return value;
        }

        private sealed class Reference : IDisposable
        {
            private readonly TKey key;
            internal readonly TValue Tracker;
            private readonly RefCountCollection<TKey, TValue> parent;
            internal int RefCount;

            public Reference(TKey key, TValue tracker, RefCountCollection<TKey, TValue> parent)
            {
                this.key = key;
                this.Tracker = tracker;
                this.parent = parent;
                this.RefCount = 1;
            }

            public void Dispose()
            {
                this.RefCount--;
                if (this.RefCount == 0)
                {
                    this.Tracker.Dispose();
                    Reference temp;
                    this.parent.items.TryRemove(this.key, out temp);
                }
            }
        }
    }
}