namespace Gu.State
{
    using System.Collections.Generic;

    internal static class Set
    {
        internal static bool TryGetComparer<T>(ISet<T> source, out IEqualityComparer<T> comparer)
        {
            if (source is HashSet<T> hashSet)
            {
                comparer = hashSet.Comparer;
                return true;
            }

            comparer = EqualityComparer<T>.Default;
            return false;
        }
    }
}
