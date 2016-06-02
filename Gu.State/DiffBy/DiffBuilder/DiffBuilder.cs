﻿namespace Gu.State
{
    using System;
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

        private bool needsRefresh;
        private bool isRefreshing;
        private bool isPurging;
        private bool isUpdatingDiffs;
        private bool isCheckingHasMemberOrIndexDiff;
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

        internal bool TryAdd(MemberInfo member, object xValue, object yValue)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                if (this.KeyedSubBuilders.ContainsKey(member))
                {
                    this.Add(member, xValue, yValue);
                    return true;
                }

                SubDiff old;
                if (!this.KeyedDiffs.TryGetValue(member, out old))
                {
                    this.Add(member, xValue, yValue);
                    return true;
                }

                if (old.Diffs.Count > 0)
                {
                    this.Add(member, xValue, yValue);
                    return true;
                }

                bool xEqual;
                bool yEqual;
                if (EqualBy.TryGetValueEquals(xValue, old.X, this.settings, out xEqual) && xEqual &&
                    EqualBy.TryGetValueEquals(yValue, old.Y, this.settings, out yEqual) && yEqual)
                {
                    return false;
                }

                this.Add(member, xValue, yValue);
                return true;
            }
        }

        internal void Add(MemberInfo member, object xValue, object yValue)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            lock (this.gate)
            {
                this.needsRefresh = true;
                this.KeyedDiffs[member] = MemberDiff.Create(member, xValue, yValue);
                this.UpdateSubBuilder(member, null);
            }
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

        internal void Refresh()
        {
            this.TryRefresh();
        }

        internal bool TryRefresh()
        {
            if (!this.needsRefresh || this.isRefreshing)
            {
                return false;
            }

            lock (this.gate)
            {
                if (!this.needsRefresh || this.isRefreshing)
                {
                    return false;
                }

                this.isRefreshing = true;
                this.Purge();
                var changed = this.TryUpdateDiffs();
                this.isRefreshing = false;
                this.needsRefresh = false;
                return changed;
            }
        }

        private void Purge()
        {
            if (!this.needsRefresh || this.isPurging)
            {
                return;
            }

            lock (this.gate)
            {
                if (!this.needsRefresh || this.isPurging)
                {
                    return;
                }

                this.isPurging = true;
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

                this.isPurging = false;
            }
        }

        private bool HasMemberOrIndexDiff()
        {
            if (this.isCheckingHasMemberOrIndexDiff)
            {
                return false;
            }

            this.isCheckingHasMemberOrIndexDiff = true;
            var result = this.KeyedDiffs.Count > this.KeyedSubBuilders.Count ||
                         this.KeyedSubBuilders.Any(kd => kd.Value.Value.HasMemberOrIndexDiff());
            this.isCheckingHasMemberOrIndexDiff = false;
            return result;
        }

        private bool TryUpdateDiffs()
        {
            if (!this.needsRefresh || this.isUpdatingDiffs)
            {
                return false;
            }

            lock (this.gate)
            {
                if (!this.needsRefresh || this.isUpdatingDiffs)
                {
                    return false;
                }

                var changed = this.diffs.Count != this.KeyedDiffs.Count;
                this.isUpdatingDiffs = true;
                foreach (var keyedSubBuilder in this.KeyedSubBuilders)
                {
                    changed |= keyedSubBuilder.Value.Value.TryUpdateDiffs();
                }

                using (var borrow = ListPool<SubDiff>.Borrow())
                {
                    foreach (var keyAndDiff in this.KeyedDiffs)
                    {
                        borrow.Value.Add(keyAndDiff.Value);
                    }

                    borrow.Value.Sort(SubDiffComparer.Default);
                    for (var index = 0; index < borrow.Value.Count; index++)
                    {
                        if (!changed)
                        {
                            changed |= !ReferenceEquals(borrow.Value[index], this.diffs[index]);
                        }

                        this.diffs.SetElementAt(index, borrow.Value[index]);
                    }

                    this.diffs.TrimLengthTo(this.KeyedDiffs.Count);
                }

                this.isUpdatingDiffs = false;
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
                throw Throw.ShouldNeverGetHereException("UpdateSubBuilder failed, try refcount failed");
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
                if (x.Index is int && y.Index is int)
                {
                    return ((int)x.Index).CompareTo(y.Index);
                }

                if ((x.X == PaddedPairs.MissingItem || x.Y == PaddedPairs.MissingItem) &&
                    (y.X == PaddedPairs.MissingItem || y.Y == PaddedPairs.MissingItem))
                {
                    var xv = (x.X == PaddedPairs.MissingItem ? 1 : 0) + (x.Y == PaddedPairs.MissingItem ? -1 : 0);
                    var yv = (y.X == PaddedPairs.MissingItem ? 1 : 0) + (y.Y == PaddedPairs.MissingItem ? -1 : 0);
                    return xv.CompareTo(yv);
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
