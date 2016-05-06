namespace Gu.State
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class RefCounted
    {
        internal static bool TryRefCount<TValue>(this TValue value, out IDisposer<TValue> disposer)
            where TValue : class, IDisposable
        {
            int count;
            disposer = RefCount<TValue>.AddOrUpdate(value, out count);
            return count > 0;
        }

        private static class RefCount<TValue>
            where TValue : class, IDisposable
        {
            private static readonly ConditionalWeakTable<TValue, RefCounter> Items = new ConditionalWeakTable<TValue, RefCounter>();

            internal static IDisposer<TValue> AddOrUpdate(TValue value, out int count)
            {
                var created = false;
                var refCounter = Items.GetValue(
                    value,
                    x =>
                        {
                            created = true;
                            return new RefCounter(x);
                        });
                if (!created)
                {
                    refCounter.Increment();
                }

                count = refCounter.Count;
                return refCounter;
            }

            private sealed class RefCounter : IDisposer<TValue>
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

                internal int Count => this.count;

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
