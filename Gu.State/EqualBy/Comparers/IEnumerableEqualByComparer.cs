namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal static class IEnumerableEqualByComparer
    {
        internal static bool TryGet(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (type.Implements(typeof(IEnumerable<>)))
            {
                comparer = (EqualByComparer)Activator.CreateInstance(
                    typeof(Comparer<>).MakeGenericType(type.GetItemType()),
                    settings.GetEqualByComparerOrDeferred(type.GetItemType()));
                return true;
            }

            comparer = null;
            return false;
        }

        private class Comparer<T> : CollectionEqualByComparer<IEnumerable<T>, T>
        {
            public Comparer(EqualByComparer itemComparer)
                : base(itemComparer)
            {
            }

            internal override bool Equals(IEnumerable<T> x, IEnumerable<T> y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
            {
                var comparer = this.ItemComparer;
                using var xe = x.GetEnumerator();
                using var ye = y.GetEnumerator();
                bool xn;
                bool yn;
                do
                {
                    xn = xe.MoveNext();
                    yn = ye.MoveNext();
                    if (xn && yn)
                    {
                        if (!comparer.Equals(xe.Current, ye.Current, settings, referencePairs))
                        {
                            return false;
                        }
                    }
                }
                while (xn && yn);

                return xn == yn;
            }
        }
    }
}