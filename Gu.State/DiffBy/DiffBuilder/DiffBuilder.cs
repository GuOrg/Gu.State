namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal sealed class DiffBuilder : IDisposable
    {
        private readonly ConcurrentDictionary<ReferencePair, DiffBuilder> builderCache;
        private readonly List<SubDiff> diffs = new List<SubDiff>();
        private readonly List<Disposer<DiffBuilder>> disposers = new List<Disposer<DiffBuilder>>();
        private readonly ValueDiff valueDiff;
        private bool disposed;

        private DiffBuilder(object x, object y, ConcurrentDictionary<ReferencePair, DiffBuilder> builderCache)
        {
            this.builderCache = builderCache;
            this.valueDiff = new ValueDiff(x, y, this.diffs);
            if (!this.builderCache.TryAdd(ReferencePair.GetOrCreate(x, y), this))
            {
                throw Throw.ShouldNeverGetHereException("Builder added twice");
            }
        }

        internal event EventHandler Empty;

        private bool IsEmpty => this.diffs.All(d => d.IsEmpty);

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            foreach (var disposer in this.disposers)
            {
                disposer.Dispose();
            }

            this.disposers.Clear();
        }

        internal static Disposer<DiffBuilder> Create(object x, object y)
        {
            var borrow = ConcurrentDictionaryPool<ReferencePair, DiffBuilder>.Borrow();
            return Disposer.Create(
                new DiffBuilder(x, y, borrow.Value),
                builder =>
                    {
                        foreach (var kvp in builder.builderCache)
                        {
                            kvp.Value.Dispose();
                        }
                        borrow.Dispose();
                    });
        }

        internal bool TryAdd(object x, object y, out DiffBuilder subDiffBuilder)
        {
            var added = false;
            subDiffBuilder = this.builderCache.GetOrAdd(
                ReferencePair.GetOrCreate(x, y),
                pair =>
                    {
                        added = true;
                        return new DiffBuilder(pair.X, pair.Y, this.builderCache);
                    });

            subDiffBuilder.Empty += this.OnSubBuilderEmpty;
            this.disposers.Add(Disposer.Create(subDiffBuilder, sdb => sdb.Empty -= this.OnSubBuilderEmpty));
            return added;
        }

        internal void Add(SubDiff subDiff)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            this.diffs.Add(subDiff);
        }

        internal void AddLazy(MemberInfo member, DiffBuilder builder)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            this.diffs.Add(MemberDiff.Create(member, builder.valueDiff));
        }

        internal void AddLazy(object index, DiffBuilder builder)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            this.diffs.Add(new IndexDiff(index, builder.valueDiff));
        }

        internal ValueDiff CreateValueDiff()
        {
            Debug.Assert(!this.disposed, "this.disposed");
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
    }
}
