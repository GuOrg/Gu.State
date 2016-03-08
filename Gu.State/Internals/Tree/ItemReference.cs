namespace Gu.State
{
    using System;

    internal struct ItemReference : IReference, IEquatable<ItemReference>
    {
        private readonly object source;
        private readonly object key;

        public ItemReference(object source)
            : this(source, null)
        {
            this.source = source;
        }

        public ItemReference(object source, object key)
        {
            this.source = source;
            this.key = key;
        }

        public static bool operator ==(ItemReference left, ItemReference right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemReference left, ItemReference right)
        {
            return !left.Equals(right);
        }

        public bool Equals(IReference other)
        {
            return this.Equals((object)other);
        }

        public bool Equals(ItemReference other)
        {
            return ReferenceEquals(this.source, other.source) && Equals(this.key, other.key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ItemReference && this.Equals((ItemReference)obj);
        }

        public override int GetHashCode()
        {
            return (this.source?.GetHashCode() ?? 0) ^ (this.key?.GetHashCode() ?? 0);
        }
    }
}