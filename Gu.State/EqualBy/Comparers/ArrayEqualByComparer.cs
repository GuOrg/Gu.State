namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal static class ArrayEqualByComparer
    {
        internal static bool TryGet(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (type.IsArray)
            {
                switch (type.GetArrayRank())
                {
                    case 1:
                        return IReadOnlyListEqualByComparer.TryCreate(type, settings, out comparer);
                    case 2:
                        comparer = (EqualByComparer)Activator.CreateInstance(
                            typeof(Comparer2D<>).MakeGenericType(type.GetItemType()),
                            settings.GetEqualByComparerOrDeferred(type.GetItemType()));
                        return true;
                    default:
                        comparer = (EqualByComparer)Activator.CreateInstance(
                            typeof(Comparer<>).MakeGenericType(type.GetItemType()),
                            settings.GetEqualByComparerOrDeferred(type.GetItemType()));
                        return true;
                }
            }

            comparer = null;
            return false;
        }

        [DebuggerDisplay("ArrayEqualByComparer<{typeof(T).PrettyName()}[,]>")]
        private class Comparer2D<T> : CollectionEqualByComparer<T[,], T>
        {
            public Comparer2D(EqualByComparer equalByComparer)
                : base(equalByComparer)
            {
            }

            internal override bool Equals(T[,] xs, T[,] ys, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
            {
                if (!Is.SameSize(xs, ys))
                {
                    return false;
                }

                var comparer = this.ItemComparer;
                for (var i = 0; i < xs.GetLength(0); i++)
                {
                    for (var j = 0; j < xs.GetLength(1); j++)
                    {
                        if (!comparer.Equals(xs[i, j], ys[i, j], settings, referencePairs))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        [DebuggerDisplay("ArrayEqualByComparer<{typeof(T).PrettyName()}[?]>")]
        private class Comparer<TItem> : CollectionEqualByComparer<Array, TItem>
        {
            private Comparer(EqualByComparer equalByComparer)
                : base(equalByComparer)
            {
            }

            internal override bool TryGetError(MemberSettings settings, out Error errors)
            {
                errors = null;
                return false;
            }

            internal override bool Equals(Array x, Array y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
            {
                if (!Is.SameSize(x, y))
                {
                    return false;
                }

                var comparer = this.ItemComparer;
                var xe = x.GetEnumerator();
                var ye = y.GetEnumerator();
                while (xe.MoveNext() && ye.MoveNext())
                {
                    if (!comparer.Equals(xe.Current, ye.Current, settings, referencePairs))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
