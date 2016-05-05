namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;

    internal static class RefCounter
    {
        internal static Disposer<TValue> RefCounted<TValue>(this TValue value)
            where TValue : class, IDisposable
        {
            return RefCount<TValue>.AddOrUpdate(value);
        }

        private static class RefCount<TValue>
            where TValue : class, IDisposable
        {
            private static readonly ConcurrentDictionary<TValue, RefCounted> Items = new ConcurrentDictionary<TValue, RefCounted>(ReferenceComparer.Default);

            public static Disposer<TValue> AddOrUpdate(TValue value)
            {
                Items.AddOrUpdate(value, v => new RefCounted(value), (_, count) => count.Increment());
                return new Disposer<TValue>(value, RemoveOwner);
            }

            private static void RemoveOwner(TValue value)
            {
                Items.TryUpdate(value, )
                owners.Value.Remove(owner);
                if (owners.Value.Count == 0)
                {
                    owners.Dispose();
                    value.Dispose();
                    Items.Remove(value);
                }
            }

            private sealed class RefCounted : IDisposable
            {
                private readonly TValue value;
                private readonly object gate = new object();
                private int count;

                public RefCounted(TValue value)
                {
                    this.value = value;
                    this.count = 1;
                }

                public void Dispose()
                {
                    lock (this.gate)
                    {
                        this.count--;
                        if (this.count > 0)
                        {
                            return;
                        }

                        RefCounted temp;
                        Items.TryRemove(this.value, out temp);
                        this.value.Dispose();
                    }
                }

                public RefCounted Increment()
                {
                    lock (this.gate)
                    {
                        Debug.Assert(this.count > 0, "this.count>0");
                        this.count++;
                        return this;
                    }
                }
            }
        }
    }
}
