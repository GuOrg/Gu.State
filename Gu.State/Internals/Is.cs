namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;

    internal static class Is
    {
        internal static bool SameSize(Array x, Array y)
        {
            if (x.Length != y.Length || x.Rank != y.Rank)
            {
                return false;
            }

            for (var i = 0; i < x.Rank; i++)
            {
                if (x.GetLowerBound(i) != y.GetLowerBound(i) ||
                    x.GetUpperBound(i) != y.GetUpperBound(i))
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool SetsOfT(object x, object y)
        {
            if (x?.GetType().Implements(typeof(ISet<>)) != true || y?.GetType().Implements(typeof(ISet<>)) != true)
            {
                return false;
            }

            return x.GetType().GetItemType() == y.GetType().GetItemType();
        }

        internal static bool ListsOfT(object x, object y)
        {
            if (x?.GetType().Implements(typeof(IList<>)) != true || y?.GetType().Implements(typeof(IList<>)) != true)
            {
                return false;
            }

            return x.GetType().GetItemType() == y.GetType().GetItemType();
        }

        internal static bool Enumerable(object source, object target)
        {
            return source is IEnumerable && target is IEnumerable;
        }

        internal static bool FixedSize(IEnumerable x, IEnumerable y)
        {
            return FixedSize(x) || FixedSize(y);
        }

        internal static bool FixedSize(object list)
        {
            return (list as IList)?.IsReadOnly == true;
        }
    }
}