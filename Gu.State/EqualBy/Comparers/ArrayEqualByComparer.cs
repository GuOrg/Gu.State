namespace Gu.State
{
    using System;

    /// <inheritdoc />
    public class ArrayEqualByComparer : EqualByComparer
    {
        public static readonly ArrayEqualByComparer Default = new ArrayEqualByComparer();

        private ArrayEqualByComparer()
        {
        }

        public static bool TryGetOrCreate(object x, object y, out EqualByComparer comparer)
        {
            if (x is Array && y is Array)
            {
                comparer = Default;
                return true;
            }

            comparer = null;
            return false;
        }

        /// <inheritdoc />
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
                           ? ItemsEquals(xl, yl, object.Equals)
                           : ItemsEquals(xl, yl, ReferenceEquals);
            }

            return isEquatable
                       ? ItemsEquals(xl, yl, object.Equals)
                       : ItemsEquals(xl, yl, (xi, yi) => compareItem(xi, yi, settings, referencePairs));
        }

        private static bool ItemsEquals(Array x, Array y, Func<object, object, bool> compare)
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