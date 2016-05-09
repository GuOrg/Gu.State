namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
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

        private bool needsRefresh;
        private bool disposed;
        private bool isRefreshing;

        private DiffBuilder(IRefCounted<ReferencePair> refCountedPair)
        {
            this.refCountedPair = refCountedPair;
            this.borrowedDiffs = DictionaryPool<object, SubDiff>.Borrow();
            this.borrowedSubBuilders = DictionaryPool<object, IRefCounted<DiffBuilder>>.Borrow();
            this.valueDiff = new ValueDiff(refCountedPair.Value.X, refCountedPair.Value.Y, this.diffs);
        }

        internal bool IsEmpty => this.diffs.All(d => d.IsEmpty);

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
                this.needsRefresh = true;
                this.KeyedDiffs[memberDiff.MemberInfo] = memberDiff;
                this.UpdateSubBuilder(memberDiff.MemberInfo, null);
            }
        }

        internal void Remove(object memberOrIndexOrKey)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.needsRefresh = true;
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
                        this.needsRefresh = true;
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
                this.needsRefresh = true;
                this.KeyedDiffs[indexDiff.Index] = indexDiff;
                this.UpdateSubBuilder(indexDiff.Index, null);
            }
        }

        internal void Add(RankDiff rankDiff)
        {
            lock (this.gate)
            {
                this.needsRefresh = true;
                this.KeyedDiffs[RankDiffKey] = rankDiff;
            }
        }

        internal void AddLazy(MemberInfo member, DiffBuilder builder)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.needsRefresh = true;
                this.KeyedDiffs[member] = MemberDiff.Create(member, builder.valueDiff);
                this.UpdateSubBuilder(member, builder);
            }
        }

        internal void AddLazy(object index, DiffBuilder builder)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.needsRefresh = true;
                this.KeyedDiffs[index] = new IndexDiff(index, builder.valueDiff);
                this.UpdateSubBuilder(index, builder);
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
            this.TryRefresh(null);
        }

        internal bool TryRefresh(IMemberSettings settings)
        {
            lock (this.gate)
            {
                if (!this.needsRefresh)
                {
                    return false;
                }

                if (this.isRefreshing)
                {
                    return false;
                }

                var changed = false;
                this.isRefreshing = true;

                foreach (var keyAndBuilder in this.KeyedSubBuilders)
                {
                    var builder = keyAndBuilder.Value.Value;
                    changed |= builder.TryRefresh(settings);
                    if (builder.IsEmpty)
                    {
                        this.KeyedDiffs.Remove(keyAndBuilder.Key);
                    }
                }

                var i = 0;
                changed |= this.diffs.Count != this.KeyedDiffs.Count;
                foreach (var keyAndDiff in this.KeyedDiffs)
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

                changed |= ((IList)this.diffs).TryTrimLengthTo(this.KeyedDiffs.Count);
                this.diffs.Sort(SubDiffComparer.Default);
                this.isRefreshing = false;
                this.needsRefresh = false;
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

        private class SubDiffComparer : IComparer<SubDiff>
        {
            public static readonly SubDiffComparer Default = new SubDiffComparer();

            private SubDiffComparer()
            {
            }

            public int Compare(SubDiff x, SubDiff y)
            {
                int result;
                if (TryCompareType(x, y, out result))
                {
                    return result;
                }

                if (TryCompare<IndexDiff>(x, y, CompareIndex, out result) ||
                    TryCompare<MemberDiff>(x, y, CompareMemberName, out result))
                {
                    return result;
                }

                return 0;
            }

            private static int CompareIndex(IndexDiff x, IndexDiff y)
            {
                var xIndex = x.Index as IComparable;
                if (xIndex != null && y.Index is IComparable)
                {
                    return xIndex.CompareTo(y.Index);
                }

                return 0;
            }

            private static int CompareMemberName(MemberDiff x, MemberDiff y)
            {
                return string.Compare(x.MemberInfo.Name, y.MemberInfo.Name, StringComparison.Ordinal);
            }

            private static bool TryCompare<T>(SubDiff x, SubDiff y, Func<T, T, int> compare, out int result)
                where T : SubDiff
            {
                if (x is T && y is T)
                {
                    result = compare((T)x, (T)y);
                    return true;
                }

                result = 0;
                return false;
            }

            private static bool TryCompareType(SubDiff x, SubDiff y, out int result)
            {
                if (x.GetType() == y.GetType())
                {
                    result = 0;
                    return false;
                }

                if (TryIs<MemberDiff, IndexDiff>(x, y, out result) ||
                    TryIs<MemberDiff, RankDiff>(x, y, out result) ||
                    TryIs<RankDiff, IndexDiff>(x, y, out result))
                {
                    result = -1;
                    return true;
                }

                result = 0;
                return true;
            }

            private static bool TryIs<T1, T2>(SubDiff x, SubDiff y, out int result)
                where T1 : SubDiff
                where T2 : SubDiff
            {
                if (x is T1 && y is T2)
                {
                    result = 1;
                    return true;
                }

                if (x is T2 && y is T1)
                {
                    result = -1;
                    return true;
                }

                result = 0;
                return false;
            }
        }
    }
}
