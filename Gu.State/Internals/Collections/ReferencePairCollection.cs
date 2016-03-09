namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal class ReferencePairCollection
    {
        private readonly ConcurrentSet<ReferencePair> pairs = new ConcurrentSet<ReferencePair>();

        internal void Add(object x, object y)
        {
            if (x == null || y == null)
            {
                return;
            }

            var type = x.GetType();
            if (type.IsValueType || type.IsEnum)
            {
                return;
            }

            this.pairs.Add(new ReferencePair(x, y));
        }

        internal bool Contains(object x, object y)
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
                return ReferenceEquals(this.X, other.X) && ReferenceEquals(this.Y, other.Y);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
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
