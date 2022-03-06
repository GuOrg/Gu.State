namespace Gu.State
{
    using System.Collections.Concurrent;

    /// <summary>
    /// A collection with reference pairs.
    /// </summary>
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public sealed class ReferencePairCollection
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        private static readonly ConcurrentQueue<ReferencePairCollection> Cache = new();
        private readonly ConcurrentSet<IRefCounted<ReferencePair>> pairs = new();

        private ReferencePairCollection()
        {
        }

        internal static IBorrowed<ReferencePairCollection> Borrow()
        {
            if (Cache.TryDequeue(out var collection))
            {
                return new Disposer<ReferencePairCollection>(collection, Return);
            }

            return new Disposer<ReferencePairCollection>(new ReferencePairCollection(), Return);
        }

        internal bool? Add(object x, object y)
        {
            if (!IsReferencePair(x, y))
            {
                return null;
            }

            var refCounted = ReferencePair.GetOrCreate(x, y);
            var added = this.pairs.Add(refCounted);
            if (!added)
            {
                refCounted.Dispose();
            }

            return added;
        }

        private static bool IsReferencePair(object x, object y)
        {
            if (x is null || y is null)
            {
                return false;
            }

            var type = x.GetType();
            if (type.IsValueType || type.IsEnum)
            {
                return false;
            }

            return true;
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
