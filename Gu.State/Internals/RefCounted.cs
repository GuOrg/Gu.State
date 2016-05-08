namespace Gu.State
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class RefCounted
    {
        internal static bool TryRefCount<TValue>(
            this TValue value,
            out IRefCounted<TValue> refCounted)
            where TValue : class, IDisposable
        {
            bool created;
            return TryRefCount(value, out refCounted, out created);
        }

        internal static bool TryRefCount<TValue>(this TValue value, out IRefCounted<TValue> refCounted, out bool created)
            where TValue : class, IDisposable
        {
            int count;
            refCounted = RefCount<TValue>.AddOrUpdate(value, out count, out created);
            return count > 0;
        }

        private static class RefCount<TValue>
            where TValue : class, IDisposable
        {
            private static readonly ConditionalWeakTable<TValue, RefCounter> Items = new ConditionalWeakTable<TValue, RefCounter>();

            internal static IRefCounted<TValue> AddOrUpdate(TValue value, out int count, out bool created)
            {
                var added = false;
                var refCounter = Items.GetValue(
                    value,
                    x =>
                        {
                            added = true;
                            return new RefCounter(x);
                        });
                if (!added)
                {
                    refCounter.Increment();
                }

                count = refCounter.Count;
                created = added;
                return refCounter;
            }

            private sealed class RefCounter : IRefCounted<TValue>
            {
                private readonly WeakReference<TValue> valueReference;
                private readonly object gate = new object();
                private int count;

                public RefCounter(TValue value)
                {
                    this.valueReference = new WeakReference<TValue>(value);
                    this.count = 1;
                }

                public TValue Value
                {
                    get
                    {
                        if (this.count == 0)
                        {
                            throw new ObjectDisposedException($"Not allowed to get the value of a {this.GetType().PrettyName()} after it is disposed.");
                        }

                        TValue value;
                        return this.valueReference.TryGetTarget(out value)
                                   ? value
                                   : null;
                    }
                }

                public int Count => this.count;

                public void Dispose()
                {
                    lock (this.gate)
                    {
                        if (this.count == 0)
                        {
                            return;
                        }

                        this.count--;
                        if (this.count > 0)
                        {
                            return;
                        }

                        TValue value;
                        if (this.valueReference.TryGetTarget(out value))
                        {
                            value.Dispose();
                        }
                    }
                }

                public RefCounter Increment()
                {
                    lock (this.gate)
                    {
                        if (this.count == 0)
                        {
                            return this;
                        }

                        this.count++;
                        return this;
                    }
                }
            }
        }
    }
}
