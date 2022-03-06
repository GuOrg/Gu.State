namespace Gu.State
{
    using System.Collections.Generic;

    internal static class ImmutableSet
    {
        internal static ImmutableSet<T> Create<T>(IEnumerable<T> items)
        {
            return ImmutableSet<T>.Create(items);
        }
    }
}
