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
        private readonly List<Factory> builders = new List<Factory>();
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

        internal bool IsEmpty => this.diffs.Any() || this.builders.Any(b => b.CanCreate);

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

        internal void AddLazy(PropertyInfo property, DiffBuilder builder)
        {
            this.builders.Add(new PropertyFactory(property, builder));
        }

        public ValueDiff CreateValueDiff()
        {
            return this.IsEmpty
                       ? null
                       : this.valueDiff.Value;
        }

        private IEnumerable<SubDiff> BuildDiffs()
        {
            return this.builders.Where(b => b.CanCreate)
                       .Select(b => b.Create());
        }

        private abstract class Factory
        {
            protected readonly DiffBuilder Builder;

            protected Factory(DiffBuilder builder)
            {
                this.Builder = builder;
            }

            public bool CanCreate => !this.Builder.IsEmpty;

            internal abstract SubDiff Create();
        }

        private class PropertyFactory : Factory
        {
            private readonly PropertyInfo propertyInfo;

            public PropertyFactory(PropertyInfo propertyInfo, DiffBuilder builder)
                : base(builder)
            {
                this.propertyInfo = propertyInfo;
            }

            internal override SubDiff Create()
            {
                return new PropertyDiff(this.propertyInfo, this.Builder.valueDiff.Value);
            }
        }
    }
}
