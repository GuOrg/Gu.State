namespace Gu.State
{
    using System;
    using System.Reflection;

    internal struct BindingFlagsAndReferenceHandling : IEquatable<BindingFlagsAndReferenceHandling>
    {
        private readonly BindingFlags bindingFlags;
        private readonly ReferenceHandling referenceHandling;

        internal BindingFlagsAndReferenceHandling(BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            this.bindingFlags = bindingFlags;
            this.referenceHandling = referenceHandling;
        }

        public static bool operator ==(BindingFlagsAndReferenceHandling left, BindingFlagsAndReferenceHandling right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BindingFlagsAndReferenceHandling left, BindingFlagsAndReferenceHandling right)
        {
            return !left.Equals(right);
        }

        public bool Equals(BindingFlagsAndReferenceHandling other) => this.bindingFlags == other.bindingFlags &&
                                                                      this.referenceHandling == other.referenceHandling;

        public override bool Equals(object obj) => obj is BindingFlagsAndReferenceHandling other &&
                                                   this.Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)this.bindingFlags * 397) ^ (int)this.referenceHandling;
            }
        }
    }
}
