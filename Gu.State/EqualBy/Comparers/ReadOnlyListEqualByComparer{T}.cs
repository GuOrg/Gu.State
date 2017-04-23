namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal class ReadOnlyListEqualByComparer<T> : EqualByComparer
    {
        public static readonly ReadOnlyListEqualByComparer<T> Default = new ReadOnlyListEqualByComparer<T>();

        private ReadOnlyListEqualByComparer()
        {
        }

        public override bool Equals(
            object x,
            object y,
            MemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            if (TryGetEitherNullEquals(x, y, out bool result))
            {
                return result;
            }

            var xl = (IReadOnlyList<T>)x;
            var yl = (IReadOnlyList<T>)y;
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
            IReadOnlyList<T> x,
            IReadOnlyList<T> y,
            MemberSettings settings,
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

        private static bool ItemsEquals(IReadOnlyList<T> x, IReadOnlyList<T> y, Func<T, T, bool> compare)
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