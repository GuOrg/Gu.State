namespace Gu.State
{
    using System.Collections.Generic;

    internal sealed class ReferenceComparer : IEqualityComparer<object>
    {
        internal static readonly ReferenceComparer Default = new();

        private ReferenceComparer()
        {
        }

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            var xPair = x as ReferencePair;
            var yPair = y as ReferencePair;
            if (xPair != null && yPair != null)
            {
                return Equals(xPair, (ReferencePair)y);
            }

            return ReferenceEquals(x, y);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        private static bool Equals(ReferencePair x, ReferencePair y)
        {
            return ReferenceEquals(x.X, y.X) && ReferenceEquals(x.Y, y.Y);
        }
    }
}
