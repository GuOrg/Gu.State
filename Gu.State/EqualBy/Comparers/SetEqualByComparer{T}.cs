namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <inheritdoc />
    public class SetEqualByComparer<T> : EqualByComparer
    {
        /// <summary>The default instance.</summary>
        public static readonly SetEqualByComparer<T> Default = new SetEqualByComparer<T>();
        private readonly SetPool pool = new SetPool();

        private SetEqualByComparer()
        {
        }

        /// <inheritdoc />
        public override bool Equals<TSetting>(
            object x,
            object y,
            Func<object, object, TSetting, ReferencePairCollection, bool> compareItem,
            TSetting settings,
            ReferencePairCollection referencePairs)
        {
            bool result;
            if (TryGetEitherNullEquals(x, y, out result))
            {
                return result;
            }

            var xs = (ISet<T>)x;
            var ys = (ISet<T>)y;
            if (xs.Count != ys.Count)
            {
                return false;
            }

            var isEquatable = settings.IsEquatable(x.GetType().GetItemType());
            var xHashSet = xs as HashSet<T>;
            if (isEquatable)
            {
                if (xHashSet?.Comparer == EqualityComparer<T>.Default)
                {
                    return xs.SetEquals(ys);
                }

                return this.ItemsEquals(xs, ys, EqualityComparer<T>.Default.Equals);
            }

            if (settings.ReferenceHandling == ReferenceHandling.References)
            {
                return this.ItemsEquals(xs, ys, (xi, yi) => ReferenceEquals(xi, yi));
            }

            var hashCodeMethod = typeof(T).GetMethod(nameof(this.GetHashCode), new Type[0]);
            if (hashCodeMethod.DeclaringType == typeof(object))
            {
                return this.ItemsEquals(xs, ys, (xi, yi) => compareItem(xi, yi, settings, referencePairs), _ => 0);
            }

            return this.ItemsEquals(xs, ys, (xi, yi) => compareItem(xi, yi, settings, referencePairs));
        }

        private bool ItemsEquals(ISet<T> x, ISet<T> y, Func<T, T, bool> compare, Func<T, int> getHashCode = null)
        {
            var set = this.pool.Borrow(compare, getHashCode);
            set.UnionWith(x);
            var result = set.SetEquals(y);
            this.pool.Return(set);
            return result;
        }

        private class SetPool
        {
            private readonly ConcurrentQueue<HashSet<T>> cache = new ConcurrentQueue<HashSet<T>>();

            internal HashSet<T> Borrow(Func<T, T, bool> compare, Func<T, int> getHashCode = null)
            {
                HashSet<T> set;
                if (this.cache.TryDequeue(out set))
                {
                    ((WrappingComparer)set.Comparer).Compare = compare;
                    ((WrappingComparer)set.Comparer).HashCode = getHashCode;
                    return set;
                }

                return new HashSet<T>(new WrappingComparer { Compare = compare, HashCode = getHashCode });
            }

            internal void Return(HashSet<T> set)
            {
                set.Clear();
                ((WrappingComparer)set.Comparer).Compare = null;
                this.cache.Enqueue(set);
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
}