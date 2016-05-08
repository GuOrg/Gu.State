namespace Gu.State
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    internal static class DictionaryPool<TKey, TValue>
    {
        private static readonly ConcurrentQueue<Dictionary<TKey, TValue>> Cache = new ConcurrentQueue<Dictionary<TKey, TValue>>();

        internal static IBorrowed<Dictionary<TKey, TValue>> Borrow()
        {
            Dictionary<TKey, TValue> dictionary;
            if (Cache.TryDequeue(out dictionary))
            {
                return Disposer.Create(dictionary, Return);
            }

            return Disposer.Create(new Dictionary<TKey, TValue>(), Return);
        }

        private static void Return(Dictionary<TKey, TValue> dictionary)
        {
            dictionary.Clear();
            Cache.Enqueue(dictionary);
        }
    }
}