namespace Gu.State
{
    using System.Collections.Concurrent;

    internal static class ConcurrentDictionaryPool<TKey, TValue>
    {
        private static readonly ConcurrentQueue<ConcurrentDictionary<TKey, TValue>> Cache = new ConcurrentQueue<ConcurrentDictionary<TKey, TValue>>();

        internal static IBorrowed<ConcurrentDictionary<TKey, TValue>> Borrow()
        {
            ConcurrentDictionary<TKey, TValue> dictionary;
            if (Cache.TryDequeue(out dictionary))
            {
                return Borrowed.Create(dictionary, Return);
            }

            return Borrowed.Create(new ConcurrentDictionary<TKey, TValue>(), Return);
        }

        private static void Return(ConcurrentDictionary<TKey, TValue> dictionary)
        {
            dictionary.Clear();
            Cache.Enqueue(dictionary);
        }
    }
}