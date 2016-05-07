namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal sealed class DiffBuilder : IDisposable
    {
        private readonly IBorrowed<Dictionary<object, SubDiff>> borrowedDiffs;
        private readonly IBorrowed<Dictionary<object, IRefCounted<DiffBuilder>>> borrowedSubBuilders;
        private readonly List<SubDiff> diffs = new List<SubDiff>();
        private readonly object gate = new object();
        private readonly ValueDiff valueDiff;
        private bool disposed;
        private bool isRefreshing;

        private DiffBuilder(object x, object y)
        {
            this.borrowedDiffs = DictionaryPool<object, SubDiff>.Borrow();
            this.borrowedSubBuilders = DictionaryPool<object, IRefCounted<DiffBuilder>>.Borrow();
            this.valueDiff = new ValueDiff(x, y, this.diffs);
        }

        private bool IsEmpty => this.borrowedDiffs.Value.Values.All(d => d.IsEmpty);

        public void Dispose()
        {
            lock (this.gate)
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;
                foreach (var disposer in this.borrowedSubBuilders.Value.Values)
                {
                    disposer.Dispose();
                }

                this.borrowedDiffs.Dispose();
                this.borrowedSubBuilders.Dispose();
            }
        }

        internal static IRefCounted<DiffBuilder> Create(object x, object y, IMemberSettings settings)
        {
            return TrackerCache.GetOrAdd(x, y, settings, pair => new DiffBuilder(pair.X, pair.Y));
        }

        internal static bool TryCreate(object x, object y, IMemberSettings settings, out IRefCounted<DiffBuilder> subDiffBuilder)
        {
            bool created;
            subDiffBuilder = TrackerCache.GetOrAdd(
                ReferencePair.GetOrCreate(x, y),
                settings,
                pair => new DiffBuilder(pair.X, pair.Y),
                out created);

            return created;
        }

        internal void Add(MemberDiff memberDiff)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.borrowedDiffs.Value.Add(memberDiff.MemberInfo, memberDiff);
            }
        }

        internal void Add(IndexDiff indexDiff)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.borrowedDiffs.Value.Add(indexDiff.Index, indexDiff);
            }
        }

        internal void Add(RankDiff rankDiff)
        {
            lock (this.gate)
            {
                this.borrowedDiffs.Value.Add(rankDiff, rankDiff);
            }
        }

        internal void AddLazy(MemberInfo member, DiffBuilder builder)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.borrowedDiffs.Value.Add(member, MemberDiff.Create(member, builder.valueDiff));
                this.AddSubBuilder(member, builder);
            }
        }

        internal void AddLazy(object index, DiffBuilder builder)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.borrowedDiffs.Value.Add(index, new IndexDiff(index, builder.valueDiff));
                this.AddSubBuilder(index, builder);
            }
        }

        internal ValueDiff CreateValueDiff()
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.Refresh();
                return this.IsEmpty
                           ? null
                           : this.valueDiff;
            }
        }

        internal void Refresh()
        {
            if (this.isRefreshing)
            {
                return;
            }

            this.isRefreshing = true;
            foreach (var keyAndBuilder in this.borrowedSubBuilders.Value)
            {
                var builder = keyAndBuilder.Value.Value;
                builder.Refresh();
                if (builder.IsEmpty)
                {
                    this.borrowedDiffs.Value.Remove(keyAndBuilder.Key);
                }
            }

            this.diffs.Clear();
            this.diffs.AddRange(this.borrowedDiffs.Value.Values);

            this.isRefreshing = false;
        }

        private void AddSubBuilder(object key, DiffBuilder builder)
        {
            IRefCounted<DiffBuilder> refCounted;
            if (!builder.TryRefCount(out refCounted))
            {
                throw Throw.ShouldNeverGetHereException("AddLazy failed, try refcount failed");
            }

            this.borrowedSubBuilders.Value.AddOrUpdate(key, refCounted);
        }
    }
}
