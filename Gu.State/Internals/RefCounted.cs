namespace Gu.State
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class RefCounted
    {
        internal static IRefCounted<T> RefCount<T>(this T item)
            where T : class, IDisposable
        {
            if (TryRefCount(item, out var result, out _))
            {
                return result;
            }

            throw Throw.ShouldNeverGetHereException($"RefCount failed for {item}");
        }

        internal static bool TryRefCount<TValue>(
            this TValue value,
            out IRefCounted<TValue> refCounted)
            where TValue : class, IDisposable
        {
            return TryRefCount(value, out refCounted, out _);
        }

        internal static bool TryRefCount<TValue>(this TValue value, out IRefCounted<TValue> refCounted, out bool created)
            where TValue : class, IDisposable
        {
            refCounted = RefCountedItem<TValue>.AddOrUpdate(value, out var count, out created);
            return count > 0;
        }

        private static class RefCountedItem<TValue>
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
                    _ = refCounter.Increment();
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

                internal RefCounter(TValue value)
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
                            throw new ObjectDisposedException($"Not allowed to get the value of a {typeof(RefCounter).PrettyName()} after it is disposed.");
                        }

                        return this.valueReference.TryGetTarget(out var value)
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

                        if (this.valueReference.TryGetTarget(out var value))
                        {
                            value.Dispose();
                        }
                    }
                }

                internal RefCounter Increment()
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
