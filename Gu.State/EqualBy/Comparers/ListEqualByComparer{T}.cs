namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    public class ListEqualByComparer<T> : EqualByComparer
    {
        public static readonly ListEqualByComparer<T> Default = new ListEqualByComparer<T>();

        private ListEqualByComparer()
        {
        }

        public override bool Equals<TSetting>(
            object x,
            object y,
            Func<object, object, TSetting, ReferencePairCollection, bool> compareItem,
            TSetting settings,
            ReferencePairCollection referencePairs)
        {
            bool result;
            if (TryGetEitherNullEquals(x, y, out result))
            {
                return result;
            }

            var xl = (IList<T>)x;
            var yl = (IList<T>)y;
            if (xl.Count != yl.Count)
            {
                return false;
            }

            return settings.IsEquatable(x.GetType().GetItemType())
                       ? this.Equals(xl, yl, EqualityComparer<T>.Default)
                       : this.Equals(xl, yl, compareItem, settings, referencePairs);
        }

        private bool Equals<TSetting>(
            IList<T> x,
            IList<T> y,
            Func<object, object, TSetting, ReferencePairCollection, bool> compareItem,
            TSetting settings,
            ReferencePairCollection referencePairs)
        {
            for (int i = 0; i < x.Count; i++)
            {
                var xv = x[i];
                var yv = y[i];
                if (referencePairs?.Contains(xv, yv) == true)
                {
                    continue;
                }

                if (!compareItem(xv, yv, settings, referencePairs))
                {
                    return false;
                }
            }

            return true;
        }

        private bool Equals(IList<T> x, IList<T> y, EqualityComparer<T> comparer)
        {
            for (int i = 0; i < x.Count; i++)
            {
                if (!comparer.Equals(x[i], y[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
