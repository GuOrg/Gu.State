namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ReferencePairCollection
    {
        private readonly HashSet<ReferencePair> pairs = new HashSet<ReferencePair>();

        public static bool IsReferenceType(object x)
        {
            if (x == null)
            {
                return false;
            }

            var type = x.GetType();
            return !EqualBy.IsEquatable(type);
        }

        public void Add(object x, object y)
        {
            this.pairs.Add(new ReferencePair(x, y));
        }

        public bool Contains(object x, object y)
        {
            return this.pairs.Contains(new ReferencePair(x, y));
        }

        private struct ReferencePair : IEquatable<ReferencePair>
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
                return Equals(left, right);
            }

            public static bool operator !=(ReferencePair left, ReferencePair right)
            {
                return !Equals(left, right);
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

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return this.Equals((ReferencePair)obj);
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
}
