namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal abstract class DiffBuilder
    {
        private readonly Dictionary<ReferencePair, DiffBuilder> builderCache;
        private readonly List<SubDiff> diffs = new List<SubDiff>();
        private readonly ValueDiff valueDiff;

        protected DiffBuilder(object x, object y, DiffBuilderRoot root)
            : this(x, y, root.builderCache)
        {
        }

        protected DiffBuilder(object x, object y, Dictionary<ReferencePair, DiffBuilder> builderCache)
        {
            this.builderCache = builderCache;
            Debug.Assert(!this.builderCache.ContainsKey(new ReferencePair(x, y)), "Builder added twice");
            this.valueDiff = new ValueDiff(x, y, this.diffs);
            this.builderCache.Add(new ReferencePair(x, y), this);
        }

        internal event EventHandler Empty;

        private bool IsEmpty => this.diffs.All(d => d.IsEmpty);

        internal abstract bool TryAdd(object x, object y, out DiffBuilder subDiffBuilder);

        internal bool TryGetBuilder(object x, object y, out DiffBuilder diffBuilder)
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
            this.diffs.Add(new PropertyDiff(property, builder.valueDiff));
        }

        internal void AddLazy(FieldInfo field, DiffBuilder builder)
        {
            this.diffs.Add(new FieldDiff(field, builder.valueDiff));
        }

        public void AddLazy(object index, DiffBuilder builder)
        {
            this.diffs.Add(new IndexDiff(index, builder.valueDiff));
        }

        public ValueDiff CreateValueDiff()
        {
            this.PurgeEmptyBuilders();
            return this.IsEmpty ? null : this.valueDiff;
        }

        protected void OnSubBuilderEmpty(object sender, EventArgs e)
        {
            var builder = (DiffBuilder)sender;
            if (this.IsEmpty)
            {
                return;
            }

            this.diffs.RemoveAll(x => x.ValueDiff == builder.valueDiff);
            if (this.IsEmpty)
            {
                this.Empty?.Invoke(this, EventArgs.Empty);
            }
        }

        private void PurgeEmptyBuilders()
        {
            foreach (var builder in this.builderCache.Values)
            {
                if (builder.IsEmpty)
                {
                    builder.Empty?.Invoke(builder, EventArgs.Empty);
                }
            }
        }
    }
}
