namespace Gu.State
{
    using System;

    public class ArrayEqualByComparer : EqualByComparer
    {
        public static readonly ArrayEqualByComparer Default = new ArrayEqualByComparer();

        private ArrayEqualByComparer()
        {
        }

        public override bool Equals<TSetting>(object x, object y, Func<object, object, TSetting, ReferencePairCollection, bool> compareItem, TSetting settings, ReferencePairCollection referencePairs)
        {
            bool result;
            if (TryGetEitherNullEquals(x, y, out result))
            {
                return result;
            }

            var xl = (Array)x;
            var yl = (Array)y;
            if (xl.Length != yl.Length || xl.Rank != yl.Rank)
            {
                return false;
            }

            for (int i = 0; i < xl.Rank; i++)
            {
                if (xl.GetLength(i) != yl.GetLength(i))
                {
                    return false;
                }
            }

            var isEquatable = settings.IsEquatable(x.GetType().GetItemType());
            if (settings.ReferenceHandling == ReferenceHandling.References)
            {
                return isEquatable
                           ? this.ItemsEquals(xl, yl, object.Equals)
                           : this.ItemsEquals(xl, yl, ReferenceEquals);
            }

            return isEquatable
                       ? this.ItemsEquals(xl, yl, object.Equals)
                       : this.ItemsEquals(xl, yl, (xi, yi) => compareItem(xi, yi, settings, referencePairs));
        }

        private bool ItemsEquals(Array x, Array y, Func<object, object, bool> compare)
        {
            var xe = x.GetEnumerator();
            var ye = y.GetEnumerator();
            while (xe.MoveNext() && ye.MoveNext())
            {
                if (!compare(xe.Current, ye.Current))
                {
                    return false;
                }
            }

            return true;
        }
    }
}