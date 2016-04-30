namespace Gu.State
{
    using System.Collections.Generic;

    internal static partial class Set
    {
        public static bool TryGetComparer<T>(ISet<T> source, out IEqualityComparer<T> comparer)
        {
            var hashSet = source as HashSet<T>;
            if (hashSet != null)
            {
                comparer = hashSet.Comparer;
                return true;
            }

            comparer = EqualityComparer<T>.Default;
            return false;
        }
    }
}