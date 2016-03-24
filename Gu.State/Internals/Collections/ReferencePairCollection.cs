namespace Gu.State
{
    using System.Collections.Generic;

    internal class ReferencePairCollection
    {
        private readonly ConcurrentSet<ReferencePair> pairs = new ConcurrentSet<ReferencePair>();

        internal void Add(object x, object y)
        {
            if (x == null || y == null)
            {
                return;
            }

            var type = x.GetType();
            if (type.IsValueType || type.IsEnum)
            {
                return;
            }

            this.pairs.Add(new ReferencePair(x, y));
        }

        internal bool Contains(object x, object y)
        {
            return this.pairs.Contains(new ReferencePair(x, y));
        }
    }
}
