namespace Gu.ChangeTracking.Tests.PropertySynchronizerStubs
{
    using System;
    using System.Collections.Generic;

    public sealed class WithGetReadOnlyPropertySealed<T> : IEquatable<WithGetReadOnlyPropertySealed<T>>
    {
        public WithGetReadOnlyPropertySealed(T value)
        {
            this.Value = value;
        }

        public T Value { get; }

        public static bool operator ==(WithGetReadOnlyPropertySealed<T> left, WithGetReadOnlyPropertySealed<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WithGetReadOnlyPropertySealed<T> left, WithGetReadOnlyPropertySealed<T> right)
        {
            return !Equals(left, right);
        }

        public bool Equals(WithGetReadOnlyPropertySealed<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return EqualityComparer<T>.Default.Equals(this.Value, other.Value);
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
            return obj is WithGetReadOnlyPropertySealed<T> && Equals((WithGetReadOnlyPropertySealed<T>)obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(this.Value);
        }
    }
}