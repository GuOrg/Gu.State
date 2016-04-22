namespace Gu.State
{
    using System.Collections.Generic;

    internal class DiffBuilderRoot : DiffBuilder
    {
        private readonly ReferenceHandling referenceHandling;
        private readonly Dictionary<ReferencePair, SubBuilder> builderCache = new Dictionary<ReferencePair, SubBuilder>();

        internal DiffBuilderRoot(ReferenceHandling referenceHandling)
        {
            this.referenceHandling = referenceHandling;
        }

        internal bool TryGetSubBuilder(object x, object y, out SubBuilder subBuilder)
        {
            var pair = new ReferencePair(x, y);
            return this.builderCache.TryGetValue(pair, out subBuilder);
        }

        internal void AddSubBuilderToCache(object x, object y, SubBuilder subBuilder)
        {
            var pair = new ReferencePair(x, y);
            this.builderCache.Add(pair, subBuilder);
        }

        internal override bool TryAdd(object x, object y, out SubBuilder subBuilder)
        {
            if (this.TryGetSubBuilder(x, y, out subBuilder))
            {
                return false;
            }

            subBuilder = new SubBuilder(this);
            this.AddSubBuilderToCache(x, y, subBuilder);
            return true;
        }
    }
}