namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;

    public sealed class ReferencePairCollection
    {
        private static readonly ConcurrentQueue<ReferencePairCollection> Cache = new ConcurrentQueue<ReferencePairCollection>();
        private readonly ConcurrentSet<ReferencePair> pairs = new ConcurrentSet<ReferencePair>();

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

        internal void Add(object x, object y)
        {
            if (x == null || y == null)
            {
                return;
            }

            var type = x.GetType();
            if (type.IsValueType || type.IsEnum)
            {
                return;
            }

            this.pairs.Add(ReferencePair.GetOrCreate(x, y));
        }

        internal bool Contains(object x, object y)
        {
            return this.pairs.Contains(ReferencePair.GetOrCreate(x, y));
        }

        private static void Return(ReferencePairCollection pairs)
        {
            pairs.pairs.Clear();
            Cache.Enqueue(pairs);
        }
    }
}
