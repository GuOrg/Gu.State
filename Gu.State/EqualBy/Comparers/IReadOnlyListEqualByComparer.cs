namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal static class IReadOnlyListEqualByComparer
    {
        internal static bool TryCreate(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (type.Implements(typeof(IReadOnlyList<>)))
            {
                if (type.IsArray)
                {
                    comparer = (EqualByComparer)Activator.CreateInstance(
                        typeof(ArrayComparer<>).MakeGenericType(type.GetItemType()),
                        settings.GetEqualByComparerOrDeferred(type.GetItemType()));
                }
                else if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    comparer = (EqualByComparer)Activator.CreateInstance(
                        typeof(ListComparer<>).MakeGenericType(type.GetItemType()),
                        settings.GetEqualByComparerOrDeferred(type.GetItemType()));
                }
                else
                {
                    comparer = (EqualByComparer)Activator.CreateInstance(
                        typeof(Comparer<,>).MakeGenericType(type, type.GetItemType()),
                        settings.GetEqualByComparerOrDeferred(type.GetItemType()));
                }

                return true;
            }

            comparer = null;
            return false;
        }

        [DebuggerDisplay("IReadOnlyListEqualByComparer<IReadOnlyListEqualByComparer<{typeof(T).PrettyName()}>>")]
        private class Comparer<TCollection, TItem> : CollectionEqualByComparer<TCollection, TItem>
            where TCollection : IReadOnlyList<TItem>
        {
            public Comparer(EqualByComparer itemComparer)
                : base(itemComparer)
            {
            }

            internal override bool Equals(TCollection x, TCollection y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
            {
                if (EqualityComparer<TCollection>.Default.Equals(x, default))
                {
                    return EqualityComparer<TCollection>.Default.Equals(y, default);
                }

                if (x.Count != y.Count)
                {
                    return false;
                }

                var comparer = this.ItemComparer;
                for (var i = 0; i < x.Count; i++)
                {
                    if (!comparer.Equals(x[i], y[i], settings, referencePairs))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        [DebuggerDisplay("ListByComparer<List<{typeof(T).PrettyName()}>>")]
        private class ListComparer<T> : CollectionEqualByComparer<List<T>, T>
        {
            public ListComparer(EqualByComparer itemComparer)
                : base(itemComparer)
            {
            }

            internal override bool Equals(List<T> x, List<T> y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
            {
                if (x.Count != y.Count)
                {
                    return false;
                }

                var comparer = this.ItemComparer;
                for (var i = 0; i < x.Count; i++)
                {
                    if (!comparer.Equals(x[i], y[i], settings, referencePairs))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        [DebuggerDisplay("ArrayEqualByComparer<{typeof(T).PrettyName()}[]>")]
        private class ArrayComparer<T> : CollectionEqualByComparer<T[], T>
        {
            public ArrayComparer(EqualByComparer itemComparer)
                : base(itemComparer)
            {
            }

            internal override bool Equals(T[] x, T[] y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
            {
                if (x.Length != y.Length)
                {
                    return false;
                }

                var comparer = this.ItemComparer;
                for (var i = 0; i < x.Length; i++)
                {
                    if (!comparer.Equals(x[i], y[i], settings, referencePairs))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
