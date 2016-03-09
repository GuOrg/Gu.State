namespace Gu.State
{
    using System.Collections.Generic;
    internal class ReferenceComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceComparer Default = new ReferenceComparer();

        private ReferenceComparer()
        {
        }

        public bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }
    }
}
