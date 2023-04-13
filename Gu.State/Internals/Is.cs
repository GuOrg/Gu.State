namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;

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

        internal static bool Enumerable(object x, object y) => x is IEnumerable && y is IEnumerable;

        internal static bool FixedSize(IEnumerable x, IEnumerable y) => FixedSize(x) || FixedSize(y);

        internal static bool FixedSize(object list) => list is IList { IsReadOnly: true };

        internal static bool Type<T>(object x, object y) => x is T && y is T;

        internal static bool SameType(object x, object y) => x?.GetType() == y?.GetType();

        internal static bool OpenGeneric(object x, object y, Type type) => OpenGeneric(x, type) && OpenGeneric(y, type);

        internal static bool OpenGeneric(object x, Type type)
        {
            Debug.Assert(type.IsInterface, "typeof(T).IsInterface add support for baseclasses?");
            return x?.GetType().Implements(type) == true;
        }

        internal static bool Equatable<T>() => typeof(T).Equatable();

        internal static bool Equatable(this Type type) => type.Implements(typeof(IEquatable<>), type);

        internal static bool Trackable<T>() => typeof(T).Equatable();

        internal static bool Trackable(Type type) =>
            typeof(INotifyPropertyChanged).IsAssignableFrom(type) ||
            typeof(INotifyCollectionChanged).IsAssignableFrom(type);

        internal static bool NotifyingCollections(object x, object y) => NotifyingCollection(x) && NotifyingCollection(y);

        internal static bool NotifyingCollection(object collection) => collection is INotifyCollectionChanged && collection is IEnumerable;
    }
}
