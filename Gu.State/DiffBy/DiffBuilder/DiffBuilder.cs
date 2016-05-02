namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal sealed class DiffBuilder
    {
        private readonly ConcurrentDictionary<ReferencePair, DiffBuilder> builderCache;
        private readonly List<SubDiff> diffs = new List<SubDiff>();
        private readonly ValueDiff valueDiff;

        private DiffBuilder(object x, object y, ConcurrentDictionary<ReferencePair, DiffBuilder> builderCache)
        {
            this.builderCache = builderCache;
            Debug.Assert(!this.builderCache.ContainsKey(new ReferencePair(x, y)), "Builder added twice");
            this.valueDiff = new ValueDiff(x, y, this.diffs);
        }

        internal event EventHandler Empty;

        private bool IsEmpty => this.diffs.All(d => d.IsEmpty);

        internal static Disposer<DiffBuilder> Borrow(object x, object y)
        {
            var builder = new DiffBuilder(x, y, new ConcurrentDictionary<ReferencePair, DiffBuilder>());
            builder.builderCache.TryAdd(new ReferencePair(x, y), builder);
            return new Disposer<DiffBuilder>(builder, _ => { });
            //// return DiffBuilderPool.Borrow(() => new DiffBuilder(x, y, new Dictionary<ReferencePair, DiffBuilder>()));
        }

        internal bool TryAdd(object x, object y, out DiffBuilder subDiffBuilder)
        {
            bool added = false;
            subDiffBuilder = this.builderCache.GetOrAdd(
                new ReferencePair(x, y),
                pair =>
                    {
                        added = true;
                        return new DiffBuilder(pair.X, pair.Y, this.builderCache);
                    });
            subDiffBuilder.Empty += this.OnSubBuilderEmpty;
            return added;
        }

        internal void Add(SubDiff subDiff)
        {
            this.diffs.Add(subDiff);
        }

        internal void AddLazy(MemberInfo member, DiffBuilder builder)
        {
            this.diffs.Add(MemberDiff.Create(member, builder.valueDiff));
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

        private void OnSubBuilderEmpty(object sender, EventArgs e)
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

        private void Clear()
        {
            this.builderCache.Clear();
        }

        internal static class DiffBuilderPool
        {
            private static readonly ConcurrentQueue<DiffBuilder> Cache = new ConcurrentQueue<DiffBuilder>();

            internal static Disposer<DiffBuilder> Borrow(Func<DiffBuilder> createNew)
            {
                DiffBuilder builder;
                if (Cache.TryDequeue(out builder))
                {
                    return Disposer.Create(builder, Return);
                }

                return Disposer.Create(createNew(), Return);
            }

            private static void Return(DiffBuilder builder)
            {
                builder.Clear();
                Cache.Enqueue(builder);
            }
        }
    }
}
