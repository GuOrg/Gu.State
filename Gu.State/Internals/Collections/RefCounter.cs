namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    internal static class RefCounter
    {
        internal static Disposer<TValue> RefCounted<TValue>(this TValue value, object owner)
            where TValue : class, IDisposable
        {
            return RefCount<TValue>.AddOrUpdate(value, owner);
        }

        private static class RefCount<TValue>
            where TValue : class, IDisposable
        {
            private static readonly Dictionary<TValue, Disposer<HashSet<object>>> Items = new Dictionary<TValue, Disposer<HashSet<object>>>(ReferenceComparer<TValue>.Default);
            private static readonly object Gate = new object();

            public static Disposer<TValue> AddOrUpdate(TValue value, object owner)
            {
                lock (Gate)
                {
                    Disposer<HashSet<object>> owners;
                    if (!Items.TryGetValue(value, out owners))
                    {
                        owners = ReferenceSetPool.Borrow();
                        Items[value] = owners;
                    }

                    if (!owners.Value.Add(owner))
                    {
                        throw Throw.ShouldNeverGetHereException("Adding owner twice");
                    }

                    return new Disposer<TValue>(value, _ => RemoveOwner(value, owner, owners));
                }
            }

            private static void RemoveOwner(TValue value, object owner, Disposer<HashSet<object>> owners)
            {
                lock (Gate)
                {
                    owners.Value.Remove(owner);
                    if (owners.Value.Count == 0)
                    {
                        owners.Dispose();
                        value.Dispose();
                        Items.Remove(value);
                    }
                }
            }
        }
    }
}