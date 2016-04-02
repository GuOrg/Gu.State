namespace Gu.State
{
    using System.Collections.Generic;

    internal static class Is
    {
        internal static bool Sets(object x, object y)
        {
            if (x?.GetType().Implements(typeof(ISet<>)) != true || y?.GetType().Implements(typeof(ISet<>)) != true)
            {
                return false;
            }

            return x.GetType().GetItemType() == y.GetType().GetItemType();
        }
    }
}