namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal abstract class DiffBuilder
    {
        private readonly Dictionary<ReferencePair, DiffBuilder> builderCache = new Dictionary<ReferencePair, DiffBuilder>();
        private readonly List<SubDiff> diffs = new List<SubDiff>();
        private readonly List<Func<SubDiff>> builders = new List<Func<SubDiff>>();
        private readonly Lazy<ValueDiff> valueDiff;
        private readonly object x;
        private readonly object y;

        protected DiffBuilder(object x, object y)
        {
            this.x = x;
            this.y = y;
            this.valueDiff = new Lazy<ValueDiff>(() => new ValueDiff(this.x, this.y, this.Diffs.ToArray()));
            Debug.Assert(!this.builderCache.ContainsKey(new ReferencePair(x, y)), "Builder added twice");
            this.builderCache.Add(new ReferencePair(x, y), this);
        }

        internal bool IsEmpty => this.diffs?.Any() != true && this.builders?.Any() == true;

        internal IEnumerable<SubDiff> Diffs => this.diffs.Concat(this.BuildDiffs());

        internal abstract bool TryAdd(object x, object y, out DiffBuilder subDiffBuilder);

        internal bool TryGetSubBuilder(object x, object y, out DiffBuilder diffBuilder)
        {
            var pair = new ReferencePair(x, y);
            return this.builderCache.TryGetValue(pair, out diffBuilder);
        }

        internal void Add(SubDiff subDiff)
        {
            this.diffs.Add(subDiff);
        }

        internal void AddLazy(Func<SubDiff> builder)
        {
            this.builders.Add(builder);
        }

        public ValueDiff CreateValueDiff()
        {
            return this.IsEmpty
                       ? null
                       : this.valueDiff.Value;
        }

        internal SubDiff CreatePropertyDiff(PropertyInfo propertyInfo)
        {
            if (this.IsEmpty)
            {
                return null;
            }

            return new PropertyDiff(propertyInfo, this.valueDiff.Value);
        }

        private IEnumerable<SubDiff> BuildDiffs()
        {
            return this.builders.Where(b => b != null)
                       .Select(b => b());
        }
    }
}
