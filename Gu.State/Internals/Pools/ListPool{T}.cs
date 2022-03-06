namespace Gu.State
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    internal static class ListPool<T>
    {
        private static readonly ConcurrentQueue<List<T>> Cache = new ConcurrentQueue<List<T>>();

        internal static IBorrowed<List<T>> Borrow()
        {
            if (Cache.TryDequeue(out var list))
            {
                return Borrowed.Create(list, Return);
            }

            return Borrowed.Create(new List<T>(), Return);
        }

        private static void Return(List<T> list)
        {
            list.Clear();
            Cache.Enqueue(list);
        }
    }
}
