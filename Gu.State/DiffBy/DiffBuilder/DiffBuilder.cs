namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal sealed class DiffBuilder : IDisposable
    {
        private static readonly object RankDiffKey = new object();
        private readonly IRefCounted<ReferencePair> refCountedPair;

        private readonly IMemberSettings settings;
        private readonly IBorrowed<Dictionary<object, SubDiff>> borrowedDiffs;
        private readonly IBorrowed<Dictionary<object, IRefCounted<DiffBuilder>>> borrowedSubBuilders;
        private readonly List<SubDiff> diffs = new List<SubDiff>();
        private readonly object gate = new object();
        private readonly ValueDiff valueDiff;

        private Status status;
        private bool disposed;

        private DiffBuilder(IRefCounted<ReferencePair> refCountedPair, IMemberSettings settings)
        {
            this.refCountedPair = refCountedPair;
            this.settings = settings;
            this.borrowedDiffs = DictionaryPool<object, SubDiff>.Borrow();
            this.borrowedSubBuilders = DictionaryPool<object, IRefCounted<DiffBuilder>>.Borrow();
            this.valueDiff = new ValueDiff(refCountedPair.Value.X, refCountedPair.Value.Y, this.diffs);
        }

        internal bool IsEmpty
        {
            get
            {
                Debug.Assert(!this.disposed, "this.disposed");
                lock (this.gate)
                {
                    this.Refresh();
                    return this.KeyedDiffs.Count == 0;
                }
            }
        }

        private string DebuggerDisplay => $"{this.GetType().Name} for {this.refCountedPair.Value.X.GetType().Name}";

        private Dictionary<object, SubDiff> KeyedDiffs => this.borrowedDiffs.Value;

        private Dictionary<object, IRefCounted<DiffBuilder>> KeyedSubBuilders => this.borrowedSubBuilders.Value;

        private bool NeedsRefresh
        {
            get { return this.status.IsFlagSet(Status.NeedsRefresh); }
            set { this.status = this.status.SetFlag(Status.NeedsRefresh, value); }
        }

        private bool IsRefreshing
        {
            get { return this.status.IsFlagSet(Status.IsRefreshing); }
            set { this.status = this.status.SetFlag(Status.IsRefreshing, value); }
        }

        private bool IsPurging
        {
            get { return this.status.IsFlagSet(Status.IsPurging); }
            set { this.status = this.status.SetFlag(Status.IsPurging, value); }
        }

        private bool IsUpdatingDiffs
        {
            get { return this.status.IsFlagSet(Status.IsUpdatingDiffs); }
            set { this.status = this.status.SetFlag(Status.IsUpdatingDiffs, value); }
        }

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

        internal static IRefCounted<DiffBuilder> GetOrCreate(object x, object y, IMemberSettings settings)
        {
            return TrackerCache.GetOrAdd(x, y, settings, pair => new DiffBuilder(pair, settings));
        }

        internal static bool TryCreate(object x, object y, IMemberSettings settings, out IRefCounted<DiffBuilder> subDiffBuilder)
        {
            bool created;
            subDiffBuilder = TrackerCache.GetOrAdd(x, y, settings, pair => new DiffBuilder(pair, settings), out created);
            return created;
        }

        internal ValueDiff CreateValueDiffOrNull()
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

        internal void Add(MemberDiff memberDiff)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.NeedsRefresh = true;
                this.KeyedDiffs[memberDiff.MemberInfo] = memberDiff;
                this.UpdateSubBuilder(memberDiff.MemberInfo, null);
            }
        }

        internal void Remove(object memberOrIndexOrKey)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.NeedsRefresh = true;
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
                        this.NeedsRefresh = true;
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
                this.NeedsRefresh = true;
                this.KeyedDiffs[indexDiff.Index] = indexDiff;
                this.UpdateSubBuilder(indexDiff.Index, null);
            }
        }

        internal void Add(RankDiff rankDiff)
        {
            lock (this.gate)
            {
                this.NeedsRefresh = true;
                this.KeyedDiffs[RankDiffKey] = rankDiff;
            }
        }

        internal void AddLazy(MemberInfo member, DiffBuilder builder)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.NeedsRefresh = true;
                this.KeyedDiffs[member] = MemberDiff.Create(member, builder.valueDiff);
                this.UpdateSubBuilder(member, builder);
            }
        }

        internal void AddLazy(object index, DiffBuilder builder)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.NeedsRefresh = true;
                this.KeyedDiffs[index] = new IndexDiff(index, builder.valueDiff);
                this.UpdateSubBuilder(index, builder);
            }
        }

        internal void Refresh()
        {
            this.TryRefresh();
        }

        internal bool TryRefresh()
        {
            if (!this.NeedsRefresh || this.IsRefreshing)
            {
                return false;
            }

            lock (this.gate)
            {
                if (!this.NeedsRefresh || this.IsRefreshing)
                {
                    return false;
                }

                this.IsRefreshing = true;
                this.Purge();
                this.UpdateDiffs();
                this.IsRefreshing = false;
                this.NeedsRefresh = false;
                return true;
            }
        }

        private void Purge()
        {
            if (!this.NeedsRefresh || this.IsPurging)
            {
                return;
            }

            lock (this.gate)
            {
                if (!this.NeedsRefresh || this.IsPurging)
                {
                    return;
                }

                this.IsPurging = true;
                using (var borrowedList = ListPool<object>.Borrow())
                {
                    borrowedList.Value.Clear();
                    foreach (var keyAndBuilder in this.KeyedSubBuilders)
                    {
                        var builder = keyAndBuilder.Value.Value;
                        builder.Purge();

                        if (!builder.HasMemberOrIndexDiff())
                        {
                            borrowedList.Value.Add(keyAndBuilder.Key);
                        }
                    }

                    foreach (var key in borrowedList.Value)
                    {
                        this.Remove(key);
                    }
                }

                this.IsPurging = false;
            }
        }

        private bool HasMemberOrIndexDiff()
        {
            return this.KeyedDiffs.Count > this.KeyedSubBuilders.Count ||
                   this.KeyedSubBuilders.Any(kd => kd.Value.Value.HasMemberOrIndexDiff());
        }

        private void UpdateDiffs()
        {
            if (!this.NeedsRefresh || this.IsUpdatingDiffs)
            {
                return;
            }

            lock (this.gate)
            {
                if (!this.NeedsRefresh || this.IsUpdatingDiffs)
                {
                    return;
                }

                this.IsUpdatingDiffs = true;
                foreach (var keyedSubBuilder in this.KeyedSubBuilders)
                {
                    keyedSubBuilder.Value.Value.UpdateDiffs();
                }

                var i = 0;
                foreach (var keyAndDiff in this.KeyedDiffs)
                {
                    this.diffs.SetElementAt(i, keyAndDiff.Value);
                    i++;
                }

                this.diffs.TrimLengthTo(this.KeyedDiffs.Count);
                this.diffs.Sort(SubDiffComparer.Default);
                this.IsUpdatingDiffs = false;
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
