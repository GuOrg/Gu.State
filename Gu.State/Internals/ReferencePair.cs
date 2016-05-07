namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal sealed class ReferencePair : IEquatable<ReferencePair>
    {
        private static readonly Dictionary<ReferencePair, ReferencePair> Cache = new Dictionary<ReferencePair, ReferencePair>();
        private static readonly object Gate = new object();
        private static readonly ReferencePair Key = new ReferencePair(new object(), new object());
        private static readonly List<ReferencePair> PurgeList = new List<ReferencePair>();

        private readonly WeakReference x;
        private readonly WeakReference y;
        private int hashCode;

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
            lock (Gate)
            {
                ReferencePair pair;
                Key.x.Target = x;
                Key.y.Target = y;
                Key.hashCode = GetHashCode(x, y);
                if (Cache.TryGetValue(Key, out pair))
                {
                    Purge();
                    return pair;
                }

                pair = new ReferencePair(x, y);
                Cache[pair] = pair;
                Purge();
                return pair;
            }
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

            if (obj.GetType() != typeof(ReferencePair))
            {
                return false;
            }

            return this.Equals((ReferencePair)obj);
        }

        public override int GetHashCode()
        {
            if (!this.IsAlive)
            {
                PurgeList.Add(this);
            }

            //// ReSharper disable once NonReadonlyMemberInGetHashCode
            return this.hashCode;
        }

        private static void Purge()
        {
            foreach (var pair in PurgeList)
            {
                Cache.Remove(pair);
            }

            PurgeList.Clear();
        }

        private static int GetHashCode(object x, object y)
        {
            unchecked
            {
                return (RuntimeHelpers.GetHashCode(x) * 397) ^ RuntimeHelpers.GetHashCode(y);
            }
        }
    }
}