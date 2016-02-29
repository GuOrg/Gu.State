namespace Gu.ChangeTracking.Tests.CopyStubs
{
    using System;

    public sealed class Immutable : IEquatable<Immutable>
    {
        public Immutable(int value)
        {
            this.Value = value;
        }

        public int Value { get;  }

        public static bool operator ==(Immutable left, Immutable right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Immutable left, Immutable right)
        {
            return !Equals(left, right);
        }

        public bool Equals(Immutable other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return this.Value == other.Value;
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
            return obj is Immutable && Equals((Immutable)obj);
        }

        public override int GetHashCode()
        {
            return this.Value;
        }
    }
}
