namespace Gu.State
{
    using System.Collections.Concurrent;

    internal static class ConcurrentQueuePool<T>
    {
        private static readonly ConcurrentQueue<ConcurrentQueue<T>> Cache = new ConcurrentQueue<ConcurrentQueue<T>>();

        internal static IBorrowed<ConcurrentQueue<T>> Borrow()
        {
            ConcurrentQueue<T> queue;
            if (Cache.TryDequeue(out queue))
            {
                return Borrowed.Create(queue, Return);
            }

            return Borrowed.Create(new ConcurrentQueue<T>(), Return);
        }

        private static void Return(ConcurrentQueue<T> queue)
        {
            T temp;
            while (queue.TryDequeue(out temp)) { }
            Cache.Enqueue(queue);
        }
    }
}