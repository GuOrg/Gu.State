namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    internal sealed class DisposingMap<TValue>
        where TValue : class, IDisposable
    {
        private static readonly ConcurrentQueue<DisposingMap<TValue>> Cache = new ConcurrentQueue<DisposingMap<TValue>>();
        private readonly Dictionary<object, TValue> items = new Dictionary<object, TValue>();
        private readonly object gate = new object();

        private DisposingMap()
        {
        }

        internal static IBorrowed<DisposingMap<TValue>> Borrow()
        {
            DisposingMap<TValue> map;
            if (Cache.TryDequeue(out map))
            {
                return new Disposer<DisposingMap<TValue>>(map, Return);
            }

            return new Disposer<DisposingMap<TValue>>(new DisposingMap<TValue>(), Return);
        }

        internal void SetValue(PropertyInfo property, TValue value)
        {
            this.SetValueCore(property, value);
        }

        internal void SetValue(int index, TValue value)
        {
            this.SetValueCore(index, value);
        }

        internal void Remove(int index)
        {
            this.RemoveCore(index);
        }

        internal void Remove(PropertyInfo property)
        {
            this.RemoveCore(property);
        }

        internal void ClearIndexTrackers()
        {
            lock (this.gate)
            {
                using (var borrow = ListPool<object>.Borrow())
                {
                    foreach (var kvp in this.items)
                    {
                        if (kvp.Key is int)
                        {
                            borrow.Value.Add(kvp.Key);
                        }
                    }

                    foreach (var key in borrow.Value)
                    {
                        this.RemoveCore(key);
                    }
                }
            }
        }

        internal void Move(int fromIndex, int toIndex)
        {
            lock (this.gate)
            {
                this.items.Move(fromIndex, toIndex);
            }
        }

        internal void Reset(IReadOnlyList<TValue> newItems)
        {
            lock (this.gate)
            {
                this.ClearIndexTrackers();
                for (var i = 0; i < newItems.Count; i++)
                {
                    this.SetValueCore(i, newItems[i]);
                }
            }
        }

        private static void Return(DisposingMap<TValue> map)
        {
            map.Clear();
            Cache.Enqueue(map);
        }

        private void SetValueCore(object key, TValue value)
        {
            if (value == null)
            {
                this.RemoveCore(key);
                return;
            }

            lock (this.gate)
            {
                this.items.AddOrUpdate(key, value);
            }
        }

        private void RemoveCore(object key)
        {
            lock (this.gate)
            {
                this.items.TryRemoveAndDispose(key);
            }
        }

        private void Clear()
        {
            lock (this.gate)
            {
                using (var borrow = ListPool<object>.Borrow())
                {
                    foreach (var kvp in this.items)
                    {
                        borrow.Value.Add(kvp.Key);
                    }

                    foreach (var key in borrow.Value)
                    {
                        this.RemoveCore(key);
                    }
                }
            }
        }
    }
}