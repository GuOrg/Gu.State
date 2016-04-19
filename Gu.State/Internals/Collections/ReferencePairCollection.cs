namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;

    public sealed class ReferencePairCollection : IDisposable
    {
        private static readonly ConcurrentQueue<ReferencePairCollection> Cache = new ConcurrentQueue<ReferencePairCollection>();
        private readonly ConcurrentSet<ReferencePair> pairs = new ConcurrentSet<ReferencePair>();

        private ReferencePairCollection()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.pairs.Clear();
            Cache.Enqueue(this);
        }

        internal static ReferencePairCollection Create()
        {
            ReferencePairCollection collection;
            if (Cache.TryDequeue(out collection))
            {
                return collection;
            }

            return new ReferencePairCollection();
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

            this.pairs.Add(new ReferencePair(x, y));
        }

        internal bool Contains(object x, object y)
        {
            return this.pairs.Contains(new ReferencePair(x, y));
        }
    }
}
