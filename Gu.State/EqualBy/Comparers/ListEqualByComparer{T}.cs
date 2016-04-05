namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;

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

            var isEquatable = settings.IsEquatable(x.GetType().GetItemType());
            if (settings.ReferenceHandling == ReferenceHandling.References)
            {
                return isEquatable
                           ? this.ItemsEquals(xl, yl, EqualityComparer<T>.Default.Equals)
                           : this.ItemsEquals(xl, yl, (xi, yi) => ReferenceEquals(xi, yi));
            }

            return isEquatable
                       ? this.ItemsEquals(xl, yl, EqualityComparer<T>.Default.Equals)
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

        private bool ItemsEquals(IList<T> x, IList<T> y, Func<T, T, bool> compare)
        {
            for (int i = 0; i < x.Count; i++)
            {
                if (!compare(x[i], y[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
