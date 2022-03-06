namespace Gu.State
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class TrackerCache
    {
        internal static IRefCounted<TValue> GetOrAdd<TKey, TValue>(
            TKey x,
            TKey y,
            MemberSettings settings,
            ConditionalWeakTable<IRefCounted<ReferencePair>, TValue>.CreateValueCallback creator)
            where TKey : class
            where TValue : class, IDisposable
        {
            return GetOrAdd(x, y, settings, creator, out _);
        }

        internal static IRefCounted<TValue> GetOrAdd<TKey, TValue>(
            TKey x,
            TKey y,
            MemberSettings settings,
            ConditionalWeakTable<IRefCounted<ReferencePair>, TValue>.CreateValueCallback creator,
            out bool created)
            where TKey : class
            where TValue : class, IDisposable
        {
            var refCounted = ReferencePair.GetOrCreate(x, y);
            var value = GetOrAdd(refCounted, settings, creator, out created);
            if (!created)
            {
                refCounted.Dispose();
            }

            return value;
        }

        internal static IRefCounted<TValue> GetOrAdd<TKey, TValue>(
            TKey key,
            MemberSettings settings,
            ConditionalWeakTable<TKey, TValue>.CreateValueCallback creator)
            where TKey : class
            where TValue : class, IDisposable
        {
            return GetOrAdd(key, settings, creator, out _);
        }

        private static IRefCounted<TValue> GetOrAdd<TKey, TValue>(
            TKey key,
            MemberSettings settings,
            ConditionalWeakTable<TKey, TValue>.CreateValueCallback creator,
            out bool created)
            where TKey : class
            where TValue : class, IDisposable
        {
            var cache = GetOrCreateCache<TKey, TValue>(settings);
            lock (cache.Gate)
            {
                var value = cache.GetOrAdd(key, creator);
                if (value.TryRefCount(out var refCounted, out created))
                {
                    if (created)
                    {
                        (value as IInitialize<TValue>)?.Initialize();
                    }

                    return refCounted;
                }

                value = creator(key);

                if (!value.TryRefCount(out refCounted, out created))
                {
                    throw Throw.ShouldNeverGetHereException("Refcounting created value failed.");
                }

                cache.Items.Remove(key);
                cache.Items.Add(key, value);
                (value as IInitialize<TValue>)?.Initialize();
                created = true;
                return refCounted;
            }
        }

        private static TypeCache<TKey, TValue> GetOrCreateCache<TKey, TValue>(MemberSettings settings)
            where TKey : class
            where TValue : class, IDisposable
        {
            return TypeCache<TKey, TValue>.SettingsCaches.GetValue(settings, _ => new TypeCache<TKey, TValue>());
        }

        private class TypeCache<TKey, TValue>
            where TKey : class
            where TValue : class
        {
            internal static readonly ConditionalWeakTable<MemberSettings, TypeCache<TKey, TValue>> SettingsCaches = new();

            internal readonly ConditionalWeakTable<TKey, TValue> Items = new();
            internal readonly object Gate = new();

            internal TValue GetOrAdd(TKey source, ConditionalWeakTable<TKey, TValue>.CreateValueCallback creator)
            {
                return this.Items.GetValue(source, creator);
            }
        }
    }
}
