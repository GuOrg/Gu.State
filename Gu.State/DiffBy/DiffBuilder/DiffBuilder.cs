namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal sealed class DiffBuilder : IDisposable
    {
        private static readonly object RankDiffKey = new object();
        private readonly IRefCounted<ReferencePair> refCountedPair;
        private readonly IBorrowed<Dictionary<object, SubDiff>> borrowedDiffs;
        private readonly IBorrowed<Dictionary<object, IRefCounted<DiffBuilder>>> borrowedSubBuilders;
        private readonly List<SubDiff> diffs = new List<SubDiff>();
        private readonly object gate = new object();
        private readonly ValueDiff valueDiff;

        private bool disposed;
        private bool isRefreshing;

        private DiffBuilder(IRefCounted<ReferencePair> refCountedPair)
        {
            this.refCountedPair = refCountedPair;
            this.borrowedDiffs = DictionaryPool<object, SubDiff>.Borrow();
            this.borrowedSubBuilders = DictionaryPool<object, IRefCounted<DiffBuilder>>.Borrow();
            this.valueDiff = new ValueDiff(refCountedPair.Value.X, refCountedPair.Value.Y, this.diffs);
        }

        internal bool IsEmpty => this.KeyedDiffs.Values.All(d => d.IsEmpty);

        private Dictionary<object, SubDiff> KeyedDiffs => this.borrowedDiffs.Value;

        private Dictionary<object, IRefCounted<DiffBuilder>> KeyedSubBuilders => this.borrowedSubBuilders.Value;

        public void Dispose()
        {
            lock (this.gate)
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;
                foreach (var disposer in this.KeyedSubBuilders.Values)
                {
                    disposer.Dispose();
                }

                this.borrowedDiffs.Dispose();
                this.borrowedSubBuilders.Dispose();
                this.refCountedPair.Dispose();
            }
        }

        internal static IRefCounted<DiffBuilder> Create(object x, object y, IMemberSettings settings)
        {
            return TrackerCache.GetOrAdd(x, y, settings, pair => new DiffBuilder(pair));
        }

        internal static bool TryCreate(object x, object y, IMemberSettings settings, out IRefCounted<DiffBuilder> subDiffBuilder)
        {
            bool created;
            subDiffBuilder = TrackerCache.GetOrAdd(x, y, settings, pair => new DiffBuilder(pair), out created);
            return created;
        }

        internal void Add(MemberDiff memberDiff)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.KeyedDiffs[memberDiff.MemberInfo] = memberDiff;
                this.UpdateSubBuilder(memberDiff.MemberInfo, null);
            }
        }

        internal void Remove(object memberOrIndexOrKey)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.KeyedDiffs.Remove(memberOrIndexOrKey);
                this.UpdateSubBuilder(memberOrIndexOrKey, null);
            }
        }

        internal void ClearIndexDiffs()
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                using (var borrowed = ListPool<object>.Borrow())
                {
                    foreach (var subDiff in this.KeyedDiffs)
                    {
                        var indexDiff = subDiff.Value as IndexDiff;
                        if (indexDiff != null)
                        {
                            borrowed.Value.Add(indexDiff.Index);
                        }
                    }

                    foreach (var index in borrowed.Value)
                    {
                        this.Remove(index);
                    }
                }
            }
        }

        internal void Add(IndexDiff indexDiff)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.KeyedDiffs[indexDiff.Index] = indexDiff;
                this.UpdateSubBuilder(indexDiff.Index, null);
            }
        }

        internal void Add(RankDiff rankDiff)
        {
            lock (this.gate)
            {
                this.KeyedDiffs[RankDiffKey] = rankDiff;
            }
        }

        internal void AddLazy(MemberInfo member, DiffBuilder builder)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.KeyedDiffs[member] = MemberDiff.Create(member, builder.valueDiff);
                this.UpdateSubBuilder(member, builder);
            }
        }

        internal void AddLazy(object index, DiffBuilder builder)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.KeyedDiffs[index] = new IndexDiff(index, builder.valueDiff);
                this.UpdateSubBuilder(index, builder);
            }
        }

        internal ValueDiff CreateValueDiff()
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.TryRefresh(null);
                return this.IsEmpty
                           ? null
                           : this.valueDiff;
            }
        }

        internal bool TryRefresh(IMemberSettings settings)
        {
            lock (this.gate)
            {
                if (this.isRefreshing)
                {
                    return false;
                }

                var changed = false;
                this.isRefreshing = true;
                var keyAndDiffs = this.borrowedDiffs.Value;
                foreach (var keyAndBuilder in this.borrowedSubBuilders.Value)
                {
                    var builder = keyAndBuilder.Value.Value;
                    changed |= builder.TryRefresh(settings);
                    if (builder.IsEmpty)
                    {
                        keyAndDiffs.Remove(keyAndBuilder.Key);
                    }
                }

                var i = 0;
                changed |= this.diffs.Count != keyAndDiffs.Count;
                foreach (var keyAndDiff in keyAndDiffs)
                {
                    if (!changed && settings != null)
                    {
                        var old = this.diffs[i];
                        bool valueEquals;
                        if (EqualBy.TryGetValueEquals(old.X, keyAndDiff.Value.X, settings, out valueEquals))
                        {
                            changed |= !valueEquals;
                        }

                        if (!changed &&
                            EqualBy.TryGetValueEquals(old.Y, keyAndDiff.Value.Y, settings, out valueEquals))
                        {
                            changed |= !valueEquals;
                        }
                    }

                    this.diffs.SetElementAt(i, keyAndDiff.Value);
                    i++;
                }

                changed |= ((IList)this.diffs).TryTrimLengthTo(keyAndDiffs.Count);
                this.isRefreshing = false;
                return changed;
            }
        }

        private void UpdateSubBuilder(object key, DiffBuilder builder)
        {
            if (builder == null)
            {
                this.KeyedSubBuilders.TryRemoveAndDispose(key);
                return;
            }

            IRefCounted<DiffBuilder> refCounted;
            bool created;
            if (!builder.TryRefCount(out refCounted, out created))
            {
                throw Throw.ShouldNeverGetHereException("AddLazy failed, try refcount failed");
            }

           this.KeyedSubBuilders.AddOrUpdate(key, refCounted);
        }
    }
}
