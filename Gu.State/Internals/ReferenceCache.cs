namespace Gu.State
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class ReferenceCache
    {
        public static IDisposer<TValue> GetOrAdd<TKey, TValue>(TKey key, Func<TValue> creator)
            where TKey : class
            where TValue : class, IDisposable
        {
            lock (TypeCache<TKey, TValue>.Gate)
            {
                var value = TypeCache<TKey, TValue>.GetOrAdd(key, creator);
                IDisposer<TValue> disposer;
                if (value.TryRefCount(out disposer))
                {
                    return disposer;
                }

                value = creator();
                if (!value.TryRefCount(out disposer))
                {
                    throw Throw.ShouldNeverGetHereException("Refcounting created value failed.");
                }

                TypeCache<TKey, TValue>.Items.Remove(key);
                TypeCache<TKey, TValue>.Items.Add(key, value);
                return disposer;
            }
        }

        private static class TypeCache<TKey, TValue>
            where TKey : class
            where TValue : class
        {
            internal static readonly ConditionalWeakTable<TKey, TValue> Items = new ConditionalWeakTable<TKey, TValue>();
            //// ReSharper disable once StaticMemberInGenericType Yes we want to lock only for TKey, TValue
            internal static readonly object Gate = new object();

            public static TValue GetOrAdd(TKey source, Func<TValue> creator)
            {
                return Items.GetValue(source, _ => creator());
            }
        }
    }
}