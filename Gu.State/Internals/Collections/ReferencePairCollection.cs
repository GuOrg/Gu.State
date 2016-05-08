namespace Gu.State
{
    using System.Collections.Concurrent;

    public sealed class ReferencePairCollection
    {
        private static readonly ConcurrentQueue<ReferencePairCollection> Cache = new ConcurrentQueue<ReferencePairCollection>();
        private readonly ConcurrentSet<IRefCounted<ReferencePair>> pairs = new ConcurrentSet<IRefCounted<ReferencePair>>();

        private ReferencePairCollection()
        {
        }

        internal static IBorrowed<ReferencePairCollection> Borrow()
        {
            ReferencePairCollection collection;
            if (Cache.TryDequeue(out collection))
            {
                return new Disposer<ReferencePairCollection>(collection, Return);
            }

            return new Disposer<ReferencePairCollection>(new ReferencePairCollection(), Return);
        }

        internal bool Add(object x, object y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            var type = x.GetType();
            if (type.IsValueType || type.IsEnum)
            {
                return false;
            }

            var refCounted = ReferencePair.GetOrCreate(x, y);
            var added = this.pairs.Add(refCounted);
            if (!added)
            {
                refCounted.Dispose();
            }

            return added;
        }

        internal bool Contains(object x, object y)
        {
            return this.pairs.Contains(ReferencePair.GetOrCreate(x, y));
        }

        private static void Return(ReferencePairCollection pairs)
        {
            foreach (var pair in pairs.pairs)
            {
                pair.Dispose();
            }

            pairs.pairs.Clear();
            Cache.Enqueue(pairs);
        }
    }
}
