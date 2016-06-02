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
            var xPair = x as ReferencePair;
            var yPair = y as ReferencePair;
            if (xPair != null && yPair != null)
            {
                return this.Equals(xPair, (ReferencePair)y);
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
