namespace Gu.State
{
    using System;
    using System.Runtime.CompilerServices;

    internal struct ReferencePairStruct : IEquatable<ReferencePairStruct>
    {
        private readonly object x;
        private readonly object y;

        private ReferencePairStruct(object x, object y)
        {
            this.x = x;
            this.y = y;
        }

        internal bool IsEmpty => this.x is null;

        public static bool operator ==(ReferencePairStruct x, ReferencePairStruct y) => x.Equals(y);

        public static bool operator !=(ReferencePairStruct x, ReferencePairStruct y) => !x.Equals(y);

        public override bool Equals(object obj)
        {
            return obj is ReferencePairStruct other &&
                   this.Equals(other);
        }

        public bool Equals(ReferencePairStruct other) => ReferenceEquals(this.x, other.x) &&
                                                         ReferenceEquals(this.y, other.y);

        public override int GetHashCode()
        {
            var hashCode = 1502939027;
            hashCode = (hashCode * -1521134295) + RuntimeHelpers.GetHashCode(this.x);
            hashCode = (hashCode * -1521134295) + RuntimeHelpers.GetHashCode(this.y);
            return hashCode;
        }

        internal static bool TryCreate<T>(T x, T y, out ReferencePairStruct result)
        {
            if (x == null ||
                y == null ||
                typeof(T).IsValueType)
            {
                result = default(ReferencePairStruct);
                return false;
            }

            result = new ReferencePairStruct(x, y);
            return true;
        }
    }
}