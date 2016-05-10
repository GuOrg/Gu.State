namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Interface methods makes more sense with IX names")]
    internal static class Is
    {
        internal static bool SameSize(Array x, Array y)
        {
            if (x.Length != y.Length || x.Rank != y.Rank)
            {
                return false;
            }

            for (var i = 0; i < x.Rank; i++)
            {
                if (x.GetLowerBound(i) != y.GetLowerBound(i) ||
                    x.GetUpperBound(i) != y.GetUpperBound(i))
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool ISetsOfT(object x, object y)
        {
            if (!OpenGeneric(x, y, typeof(ISet<>)))
            {
                return false;
            }

            return x.GetType().GetItemType() == y.GetType().GetItemType();
        }

        internal static bool IListsOfT(object x, object y)
        {
            if (!OpenGeneric(x, y, typeof(IList<>)))
            {
                return false;
            }

            return x.GetType().GetItemType() == y.GetType().GetItemType();
        }

        internal static bool IDictionaryOfTKeyTValue(object x, object y)
        {
            if (!OpenGeneric(x, y, typeof(IDictionary<,>)))
            {
                return false;
            }

            var xArgs = x.GetType().GenericTypeArguments;
            var yArgs = y.GetType().GenericTypeArguments;
            return xArgs[0] == yArgs[0] && xArgs[1] == yArgs[1];
        }

        internal static bool Enumerable(object x, object y)
        {
            return x is IEnumerable && y is IEnumerable;
        }

        internal static bool IsFixedSize(IEnumerable x, IEnumerable y)
        {
            return IsFixedSize(x) || IsFixedSize(y);
        }

        internal static bool IsFixedSize(object list)
        {
            return (list as IList)?.IsReadOnly == true;
        }

        internal static bool Type<T>(object x, object y)
        {
            return x is T && y is T;
        }

        internal static bool OpenGeneric(object x, object y, Type type)
        {
            return OpenGeneric(x, type) && OpenGeneric(y, type);
        }

        internal static bool OpenGeneric(object x, Type type)
        {
            Debug.Assert(type.IsInterface, "typeof(T).IsInterface add support for baseclasses?");
            return x?.GetType().Implements(type) == true;
        }

        internal static bool IsEquatable<T>()
        {
            return typeof(T).IsEquatable();
        }

        internal static bool IsEquatable(this Type type)
        {
            return type.Implements(typeof(IEquatable<>), type);
        }

        internal static bool NotifyCollections(object x, object y)
        {
            return NotifyCollection(x) && NotifyCollection(y);
        }

        internal static bool NotifyCollection(object collection)
        {
            return collection is INotifyCollectionChanged && collection is IList;
        }
    }
}