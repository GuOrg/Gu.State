namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    /// <inheritdoc />
    internal class EnumerableEqualByComparer<T> : EqualByComparer
    {
        /// <summary>The default instance.</summary>
        public static readonly EnumerableEqualByComparer<T> Default = new EnumerableEqualByComparer<T>();

        private EnumerableEqualByComparer()
        {
        }

        /// <inheritdoc />
        public override bool Equals(
            object x,
            object y,
            MemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            bool result;
            if (TryGetEitherNullEquals(x, y, out result))
            {
                return result;
            }

            var isEquatable = settings.IsEquatable(typeof(T));
            if (settings.ReferenceHandling == ReferenceHandling.References)
            {
                return isEquatable
                           ? ItemsEquals((IEnumerable<T>)x, (IEnumerable<T>)y, EqualityComparer<T>.Default.Equals)
                           : ItemsEquals((IEnumerable<T>)x, (IEnumerable<T>)y, (xi, yi) => ReferenceEquals(xi, yi));
            }

            return isEquatable
                       ? ItemsEquals((IEnumerable<T>)x, (IEnumerable<T>)y, EqualityComparer<T>.Default.Equals)
                       : ItemsEquals((IEnumerable<T>)x, (IEnumerable<T>)y, (xi, yi) => EqualBy.MemberValues(xi, yi, settings, referencePairs));
        }

        private static bool ItemsEquals(IEnumerable<T> x, IEnumerable<T> y, Func<T, T, bool> compare)
        {
            using (var xe = x.GetEnumerator())
            {
                using (var ye = y.GetEnumerator())
                {
                    while (true)
                    {
                        var xn = xe.MoveNext();
                        var yn = ye.MoveNext();
                        if (xn && yn)
                        {
                            if (!compare(xe.Current, ye.Current))
                            {
                                return false;
                            }

                            continue;
                        }

                        return !xn && !yn;
                    }
                }
            }
        }
    }
}
