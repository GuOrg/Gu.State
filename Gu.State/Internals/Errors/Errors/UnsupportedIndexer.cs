namespace Gu.State
{
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("{GetType().Name} Indexer: {Indexer.Name}")]
    internal sealed class UnsupportedIndexer : Error, IExcludableMember, INotsupportedMember
    {
        private static readonly ConcurrentDictionary<PropertyInfo, UnsupportedIndexer> Cache = new();

        private UnsupportedIndexer(PropertyInfo indexer)
        {
            this.Indexer = indexer;
        }

        public MemberInfo Member => this.Indexer;

        internal PropertyInfo Indexer { get; }

        internal static UnsupportedIndexer GetOrCreate(PropertyInfo indexer)
        {
            Debug.Assert(indexer.GetIndexParameters().Length > 0, "Must be an indexer");
            return Cache.GetOrAdd(indexer, i => new UnsupportedIndexer(i));
        }
    }
}
