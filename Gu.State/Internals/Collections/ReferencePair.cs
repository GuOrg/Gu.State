namespace Gu.State
{
    using System;

    internal struct ReferencePair : IReference, IEquatable<ReferencePair>
    {
        internal readonly object X;
        internal readonly object Y;

        public ReferencePair(object x, object y)
        {
            this.X = x;
            this.Y = y;
        }

        public static bool operator ==(ReferencePair left, ReferencePair right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReferencePair left, ReferencePair right)
        {
            return !left.Equals(right);
        }

        public bool Equals(ReferencePair other)
        {
            return ReferenceEquals(this.X, other.X) && ReferenceEquals(this.Y, other.Y);
        }

        public bool Equals(IReference other)
        {
           return this.Equals((object)other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ReferencePair && this.Equals((ReferencePair)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.X?.GetHashCode() ?? 0) * 397) ^ (this.Y?.GetHashCode() ?? 0);
            }
        }
    }
}