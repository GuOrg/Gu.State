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

        public override bool Equals(
            object x,
            object y,
            IMemberSettings settings,
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
                           ? ItemsEquals(xl, yl, EqualityComparer<T>.Default.Equals)
                           : ItemsEquals(xl, yl, (xi, yi) => ReferenceEquals(xi, yi));
            }

            return isEquatable
                       ? ItemsEquals(xl, yl, EqualityComparer<T>.Default.Equals)
                       : Equals(xl, yl, settings, referencePairs);
        }

        private static bool Equals(
            IList<T> x,
            IList<T> y,
            IMemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            for (var i = 0; i < x.Count; i++)
            {
                var xv = x[i];
                var yv = y[i];
                if (referencePairs?.Contains(xv, yv) == true)
                {
                    continue;
                }

                if (!EqualBy.MemberValues(xv, yv, settings, referencePairs))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ItemsEquals(IList<T> x, IList<T> y, Func<T, T, bool> compare)
        {
            for (var i = 0; i < x.Count; i++)
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
