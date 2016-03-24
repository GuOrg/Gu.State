namespace Gu.State
{
    using System.Collections.Generic;

    internal class ReferenceComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceComparer Default = new ReferenceComparer();

        private ReferenceComparer()
        {
        }

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            if (x is ReferencePair && y is ReferencePair)
            {
                return this.Equals((ReferencePair)x, (ReferencePair)y);
            }

            return ReferenceEquals(x, y);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        private bool Equals(ReferencePair x, ReferencePair y)
        {
            return ReferenceEquals(x.X, y.X) && ReferenceEquals(x.Y, y.Y);
        }
    }
}
