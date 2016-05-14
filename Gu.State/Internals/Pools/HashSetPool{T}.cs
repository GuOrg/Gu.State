namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal static class HashSetPool<T>
    {
        private static readonly ConcurrentQueue<HashSet<T>> Cache = new ConcurrentQueue<HashSet<T>>();

        internal static IBorrowed<HashSet<T>> Borrow(IEqualityComparer<T> comparer)
        {
            return Borrow(comparer.Equals, comparer.GetHashCode);
        }

        internal static IBorrowed<HashSet<T>> Borrow(Func<T, T, bool> compare, Func<T, int> getHashCode)
        {
            Debug.Assert(compare != null, "compare == null");
            Debug.Assert(getHashCode != null, "getHashCode == null");

            HashSet<T> set;
            if (Cache.TryDequeue(out set))
            {
                ((WrappingComparer)set.Comparer).Compare = compare;
                ((WrappingComparer)set.Comparer).HashCode = getHashCode;
                return Borrowed.Create(set, Return);
            }

            set = new HashSet<T>(new WrappingComparer { Compare = compare, HashCode = getHashCode });
            return Borrowed.Create(set, Return);
        }

        private static void Return(HashSet<T> set)
        {
            set.Clear();
            ((WrappingComparer)set.Comparer).Compare = null;
            Cache.Enqueue(set);
        }

        private class WrappingComparer : IEqualityComparer<T>
        {
            public Func<T, int> HashCode { private get; set; }

            public Func<T, T, bool> Compare { private get; set; }

            public bool Equals(T x, T y) => this.Compare(x, y);

            public int GetHashCode(T obj) => this.HashCode?.Invoke(obj) ?? obj.GetHashCode();
        }
    }
}