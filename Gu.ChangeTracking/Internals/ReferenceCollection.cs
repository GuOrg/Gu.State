namespace Gu.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;

    public class ReferenceCollection
    {
        private readonly List<ReferencePair> pairs = new List<ReferencePair>();

        public static bool IsReferenceType(object x)
        {
            if (x == null)
            {
                return false;
            }

            var type = x.GetType();
            return !EqualBy.IsEquatable(type);
        }

        public void Add(object x, object y)
        {
            this.pairs.Add(new ReferencePair(x, y));
        }

        public bool Contains(object x, object y)
        {
            return this.pairs.Any(p => p.Equals(x, y));
        }

        private class ReferencePair
        {
            internal readonly object X;
            internal readonly object Y;

            public ReferencePair(object x, object y)
            {
                this.X = x;
                this.Y = y;
            }

            public new bool Equals(object x, object y)
            {
                return ReferenceEquals(this.X, x) && ReferenceEquals(this.Y, y);
            }
        }
    }
}
