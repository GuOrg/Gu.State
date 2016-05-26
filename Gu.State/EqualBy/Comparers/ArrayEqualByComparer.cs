namespace Gu.State
{
    using System;

    /// <inheritdoc />
    internal class ArrayEqualByComparer : EqualByComparer
    {
        /// <summary>The default instance.</summary>
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

            return Equals((Array)x, (Array)y, settings, referencePairs);
        }

        private static bool Equals(
            Array x,
            Array y,
            IMemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            if (!Is.SameSize(x, y))
            {
                return false;
            }

            var isEquatable = settings.IsEquatable(x.GetType().GetItemType());
            if (settings.ReferenceHandling == ReferenceHandling.References)
            {
                return isEquatable
                           ? ItemsEquals(x, y, Equals)
                           : ItemsEquals(x, y, ReferenceEquals);
            }

            return isEquatable
                       ? ItemsEquals(x, y, Equals)
                       : ItemsEquals(x, y, (xi, yi) => EqualBy.MemberValues(xi, yi, settings, referencePairs));
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