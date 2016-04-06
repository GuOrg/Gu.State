namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    public class EnumerableEqualByComparer<T> : EqualByComparer
    {
        public static readonly EnumerableEqualByComparer<T> Default = new EnumerableEqualByComparer<T>();

        private EnumerableEqualByComparer()
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

            var isEquatable = settings.IsEquatable(typeof(T));
            if (settings.ReferenceHandling == ReferenceHandling.References)
            {
                return isEquatable
                           ? ItemsEquals((IEnumerable<T>)x, (IEnumerable<T>)y, EqualityComparer<T>.Default.Equals)
                           : ItemsEquals((IEnumerable<T>)x, (IEnumerable<T>)y, (xi, yi) => ReferenceEquals(xi, yi));
            }

            return isEquatable
                       ? ItemsEquals((IEnumerable<T>)x, (IEnumerable<T>)y, EqualityComparer<T>.Default.Equals)
                       : ItemsEquals((IEnumerable<T>)x, (IEnumerable<T>)y, (xi, yi) => compareItem(xi, yi, settings, referencePairs));
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
