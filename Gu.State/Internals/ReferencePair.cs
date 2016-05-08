namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal sealed class ReferencePair
    {
        private static readonly PairCache Cache = new PairCache();

        private readonly WeakReference x;
        private readonly WeakReference y;
        private readonly int hashCode;

        private ReferencePair(object x, object y)
        {
            Debug.Assert(x != null, "x == null");
            Debug.Assert(y != null, "y == null");
            this.x = new WeakReference(x);
            this.y = new WeakReference(y);
            this.hashCode = GetHashCode(x, y);
        }

        public object X => this.x.Target;

        public object Y => this.y.Target;

        private bool IsAlive => this.x.IsAlive && this.y.IsAlive;

        public static bool operator ==(ReferencePair left, ReferencePair right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ReferencePair left, ReferencePair right)
        {
            return !Equals(left, right);
        }

        public static ReferencePair GetOrCreate<T>(T x, T y)
            where T : class
        {
            var pair = Cache.GetValue(x, y);
            return pair;
        }

        public bool Equals(ReferencePair other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ReferenceEquals(this.X, other.X) && ReferenceEquals(this.Y, other.Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var other = obj as ReferencePair;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        private static int GetHashCode(object x, object y)
        {
            unchecked
            {
                return (RuntimeHelpers.GetHashCode(x) * 397) ^ RuntimeHelpers.GetHashCode(y);
            }
        }

        private class PairCache
        {
            private readonly ConcurrentDictionary<ReferencePair, ReferencePair> map;
            private readonly List<ReferencePair> purgeList = new List<ReferencePair>();

            private bool isPurging;

            public PairCache()
            {
                this.map = new ConcurrentDictionary<ReferencePair, ReferencePair>(new PurgingComparer(this));
            }

            public ReferencePair GetValue(object xKey, object yKey)
            {
                var pair = this.map.GetOrAdd(new ReferencePair(xKey, yKey), referencePair => referencePair);

                if (this.purgeList.Count > 0)
                {
                    lock (this.purgeList)
                    {
                        if (this.purgeList.Count > 0)
                        {
                            this.isPurging = true;
                            for (int i = this.purgeList.Count - 1; i >= 0; i--)
                            {
                                ReferencePair temp;
                                this.map.TryRemove(this.purgeList[i], out temp);
                                this.purgeList.RemoveAt(i);
                            }
                        }
                    }
                }

                this.isPurging = false;
                return pair;
            }

            private class PurgingComparer : IEqualityComparer<ReferencePair>
            {
                private readonly PairCache cache;

                public PurgingComparer(PairCache cache)
                {
                    this.cache = cache;
                }

                public bool Equals(ReferencePair x, ReferencePair y)
                {
                    if (this.TryAddToPurgeList(x) || this.TryAddToPurgeList(y))
                    {
                        return false;
                    }

                    return x.Equals(y);
                }

                public int GetHashCode(ReferencePair obj)
                {
                    this.TryAddToPurgeList(obj);
                    return obj.GetHashCode();
                }

                private bool TryAddToPurgeList(ReferencePair pair)
                {
                    if (this.cache.isPurging || pair.IsAlive)
                    {
                        return false;
                    }

                    lock (this.cache.purgeList)
                    {
                        this.cache.purgeList.Add(pair);
                    }

                    return true;
                }
            }
        }
    }
}