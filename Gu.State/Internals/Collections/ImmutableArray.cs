namespace Gu.State
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class ImmutableArray
    {
        internal static ImmutableArray<T> Create<T>(IEnumerable<T> items)
        {
            return new ImmutableArray<T>(items as T[] ?? items.ToArray());
        }
    }
}
