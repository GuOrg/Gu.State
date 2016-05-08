namespace Gu.State
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal sealed class ReferencePair
    {
        private static readonly ConditionalWeakTable2D<object, ReferencePair> Cache = new ConditionalWeakTable2D<object, ReferencePair>();

        private readonly WeakReference x;
        private readonly WeakReference y;

        private ReferencePair(object x, object y)
        {
            Debug.Assert(x != null, "x == null");
            Debug.Assert(y != null, "y == null");
            this.x = new WeakReference(x);
            this.y = new WeakReference(y);
        }

        public object X => this.x.Target;

        public object Y => this.y.Target;

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
            var pair = Cache.GetValue(x, y, () => new ReferencePair(x, y));
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
            return GetHashCode(this.X, this.Y);
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