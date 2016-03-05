namespace Gu.ChangeTracking
{
    using System;
    using System.Diagnostics;
    using System.Text;

    [DebuggerDisplay("{GetType().Name} Type: {Type}")]
    internal sealed class RequiresReferenceHandling : TypeError, IFixWithEquatable, IExcludableType
    {
        public RequiresReferenceHandling(Type type)
            : base(type)
        {
        }

        Type IExcludableType.Type => this.Type;

        public static bool operator ==(RequiresReferenceHandling left, RequiresReferenceHandling right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RequiresReferenceHandling left, RequiresReferenceHandling right)
        {
            return !Equals(left, right);
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

            return this.Equals((RequiresReferenceHandling)obj);
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }

        private bool Equals(RequiresReferenceHandling other)
        {
            return this.Type == other.Type;
        }
    }
}