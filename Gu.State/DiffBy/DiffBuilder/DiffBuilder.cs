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
        private readonly ValueDiff valueDiff;
        private bool disposed;
        private Disposer<ConcurrentDictionary<DiffBuilder, Disposer<HashSet<object>>>> borrowedDictionary;

        private DiffBuilder(object x, object y, ConcurrentDictionary<ReferencePair, DiffBuilder> builderCache)
        {
            this.builderCache = builderCache;
            this.valueDiff = new ValueDiff(x, y, this.diffs);
            if (!this.builderCache.TryAdd(new ReferencePair(x, y), this))
            {
                throw Throw.ShouldNeverGetHereException("Duplicate builder for reference pair");
            }

            this.borrowedDictionary = ConcurrentDictionaryPool<DiffBuilder, Disposer<HashSet<object>>>.Borrow();
        }

        internal event EventHandler Empty;

        private bool IsEmpty => this.diffs.All(d => d.IsEmpty);

        private ConcurrentDictionary<DiffBuilder, Disposer<HashSet<object>>> SubBuilders => this.borrowedDictionary.Value;

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            foreach (var builder in this.SubBuilders)
            {
                builder.Key.Empty -= this.OnSubBuilderEmpty;
                builder.Value.Dispose();
            }

            this.borrowedDictionary.Dispose();
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

        internal bool TryCreate(object x, object y, out DiffBuilder subDiffBuilder)
        {
            var added = false;
            subDiffBuilder = this.builderCache.GetOrAdd(
                new ReferencePair(x, y),
                pair =>
                    {
                        added = true;
                        return new DiffBuilder(pair.X, pair.Y, this.builderCache);
                    });
            return added;
        }

        internal void Add(SubDiff subDiff)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            this.diffs.Add(subDiff);
        }

        internal void Add(MemberInfo member, DiffBuilder builder)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            this.diffs.Add(MemberDiff.Create(member, builder.valueDiff));
            this.AddSubBuilder(member, builder);
        }

        internal void Add(object index, DiffBuilder builder)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            this.diffs.Add(new IndexDiff(index, builder.valueDiff));
            this.AddSubBuilder(index, builder);
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

        private void AddSubBuilder(object index, DiffBuilder builder)
        {
            this.SubBuilders.AddOrUpdate(
                builder,
                b =>
                {
                    b.Empty += this.OnSubBuilderEmpty;
                    var borrowedSet = SetPool<object>.Borrow(EqualityComparer<object>.Default);
                    borrowedSet.Value.Add(index);
                    return borrowedSet;
                },
                (b, s) =>
                {
                    s.Value.Add(index);
                    return s;
                });
        }
    }
}
