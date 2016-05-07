namespace Gu.State
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class TrackerCache
    {
        internal static IRefCounted<TValue> GetOrAdd<TKey, TValue>(
            TKey x,
            TKey y,
            IMemberSettings settings,
            ConditionalWeakTable<ReferencePair, TValue>.CreateValueCallback creator)
            where TKey : class
            where TValue : class, IDisposable
        {
            return GetOrAdd(ReferencePair.GetOrCreate(x, y), settings, creator);
        }

        internal static IRefCounted<TValue> GetOrAdd<TKey, TValue>(
            TKey x,
            TKey y,
            IMemberSettings settings,
            ConditionalWeakTable<ReferencePair, TValue>.CreateValueCallback creator,
            out bool created) 
            where TKey : class 
            where TValue : class, IDisposable
        {
            return GetOrAdd(ReferencePair.GetOrCreate(x, y), settings, creator, out created);
        }

        internal static IRefCounted<TValue> GetOrAdd<TKey, TValue>(
            TKey key,
            IMemberSettings settings,
            ConditionalWeakTable<TKey, TValue>.CreateValueCallback creator)
            where TKey : class
            where TValue : class, IDisposable
        {
            bool temp;
            return GetOrAdd(key, settings, creator, out temp);
        }

        internal static IRefCounted<TValue> GetOrAdd<TKey, TValue>(
            TKey key,
            IMemberSettings settings,
            ConditionalWeakTable<TKey, TValue>.CreateValueCallback creator,
            out bool created)
            where TKey : class
            where TValue : class, IDisposable
        {
            var cache = GetOrCreateCache<TKey, TValue>(settings);
            lock (cache.Gate)
            {
                var value = cache.GetOrAdd(key, creator);
                IRefCounted<TValue> disposer;
                if (value.TryRefCount(out disposer))
                {
                    created = false;
                    return disposer;
                }

                value = creator(key);
                if (!value.TryRefCount(out disposer))
                {
                    throw Throw.ShouldNeverGetHereException("Refcounting created value failed.");
                }

                cache.Items.Remove(key);
                cache.Items.Add(key, value);
                created = true;
                return disposer;
            }
        }

        private static TypeCache<TKey, TValue> GetOrCreateCache<TKey, TValue>(IMemberSettings settings)
            where TKey : class
            where TValue : class, IDisposable
        {
            return TypeCache<TKey, TValue>.SettingsCaches.GetValue(settings, _ => new TypeCache<TKey, TValue>());
        }

        private class TypeCache<TKey, TValue>
            where TKey : class
            where TValue : class
        {
            internal static readonly ConditionalWeakTable<IMemberSettings, TypeCache<TKey, TValue>> SettingsCaches = new ConditionalWeakTable<IMemberSettings, TypeCache<TKey, TValue>>();

            internal readonly ConditionalWeakTable<TKey, TValue> Items = new ConditionalWeakTable<TKey, TValue>();
            internal readonly object Gate = new object();

            internal TValue GetOrAdd(TKey source, ConditionalWeakTable<TKey, TValue>.CreateValueCallback creator)
            {
                return this.Items.GetValue(source, creator);
            }
        }
    }
}