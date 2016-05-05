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
        private readonly ValueDiff valueDiff;
        private readonly ConcurrentDictionary<ReferencePair, DiffBuilder> builderCache;
        private readonly List<SubDiff> diffs = new List<SubDiff>();
        private bool disposed;
        private Disposer<ConcurrentDictionary<DiffBuilder, Disposer<HashSet<object>>>> subBilderMemberOrItemMap;

        private DiffBuilder(object x, object y, ConcurrentDictionary<ReferencePair, DiffBuilder> builderCache)
        {
            this.builderCache = builderCache;
            this.valueDiff = new ValueDiff(x, y, this.diffs);
            if (!this.builderCache.TryAdd(new ReferencePair(x, y), this))
            {
                throw Throw.ShouldNeverGetHereException("Duplicate builder for reference pair");
            }

            this.subBilderMemberOrItemMap = ConcurrentDictionaryPool<DiffBuilder, Disposer<HashSet<object>>>.Borrow();
        }

        internal event EventHandler Empty;

        private bool IsEmpty => this.diffs.All(d => d.IsEmpty);

        private ConcurrentDictionary<DiffBuilder, Disposer<HashSet<object>>> SubBuilders => this.subBilderMemberOrItemMap.Value;

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

            this.subBilderMemberOrItemMap.Dispose();
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

        internal bool TryAddOrUpdate(MemberDiff memberDiff)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.diffs)
            {
                var index = this.IndexOf(memberDiff.Member);
                if (index < 0)
                {
                    this.diffs.Add(memberDiff);
                    return true;
                }
                else
                {
                    var old = this.diffs[index];
                    this.diffs[index] = memberDiff;
                    return !Equals(old.X, memberDiff.X) || !Equals(old.Y, memberDiff.Y);
                }
            }
        }

        internal bool TryRemove(MemberInfo member)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.diffs)
            {
                throw new NotImplementedException("Must remove and dispose builder here if needed");
                var index = this.IndexOf(member);
                if (index >= 0)
                {
                    this.diffs.RemoveAt(index);
                    return true;
                }

                return false;
            }
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

        internal void PurgeEmptyBuilders()
        {
            foreach (var builder in this.builderCache.Values)
            {
                if (builder.IsEmpty)
                {
                    builder.Empty?.Invoke(builder, EventArgs.Empty);
                }
            }
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

        private void AddSubBuilder(object key, DiffBuilder builder)
        {
            this.SubBuilders.AddOrUpdate(
                builder,
                b =>
                {
                    b.Empty += this.OnSubBuilderEmpty;
                    var borrowedSet = SetPool<object>.Borrow(EqualityComparer<object>.Default);
                    borrowedSet.Value.Add(key);
                    return borrowedSet;
                },
                (b, s) =>
                {
                    s.Value.Add(key);
                    return s;
                });
        }

        private int IndexOf(MemberInfo member)
        {
            for (int i = 0; i < this.diffs.Count; i++)
            {
                var memberDiff = this.diffs[i] as MemberDiff;
                if (memberDiff != null && memberDiff.Member == member)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
