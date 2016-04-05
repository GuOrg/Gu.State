namespace Gu.State
{
    using System;

    public abstract class EqualByComparer
    {
        public abstract bool Equals<TSetting>(
            object x,
            object y,
            Func<object, object, TSetting, ReferencePairCollection, bool> compareItem,
            TSetting settings,
            ReferencePairCollection referencePairs)
            where TSetting : class, IMemberSettings;

        protected static bool TryGetEitherNullEquals(object x, object y, out bool result)
        {
            if (x == null && y == null)
            {
                result = true;
                return true;
            }

            if (x == null || y == null)
            {
                result = false;
                return true;
            }

            result = false;
            return false;
        }
    }
}
