namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class Set
    {
        private static ConcurrentDictionary<Type, MethodInfo> EmptyMethods = new ConcurrentDictionary<Type, MethodInfo>();
        private static ConcurrentDictionary<Type, MethodInfo> UnionWithMethods = new ConcurrentDictionary<Type, MethodInfo>();
        private static ConcurrentDictionary<Type, MethodInfo> IntersectWithMethods = new ConcurrentDictionary<Type, MethodInfo>();

        internal static bool Add(object set, object value)
        {
            var addMethod = set.GetType().GetMethod("Add", new[] { set.GetType().GetItemType() });
            return (bool)addMethod.Invoke(set, new[] { value });
        }

        internal static IEnumerable<object> ElementsOrderedByHashCode(IEnumerable set)
        {
            var comparer = set.GetType()
                              .GetProperty("Comparer", Constants.DefaultPropertyBindingFlags)
                              ?.GetValue(set);
            if (comparer == null)
            {
                return set.Cast<object>()
                          .OrderBy(i => i.GetHashCode());
            }

            var getHashcodeMethod = comparer.GetType()
                                            .GetMethod("GetHashCode", new[] { set.GetType().GetItemType() });
            return set.Cast<object>()
                      .OrderBy(i => getHashcodeMethod.Invoke(comparer, new[] { i }));
        }

        internal static void UnionWith(object set, object otherSet)
        {
            var unitonWithMethod = UnionWithMethods.GetOrAdd(set.GetType(), GetUnionWithMethod);
            unitonWithMethod.Invoke(set, new[] { otherSet });
        }

        internal static void IntersectWith(object set, object otherSet)
        {
            var intersectMethod = IntersectWithMethods.GetOrAdd(set.GetType(), GetIntersectWithMethod);
            intersectMethod.Invoke(set, new[] { otherSet });
        }

        internal static void Clear(object source)
        {
            var emptyMethod = EmptyMethods.GetOrAdd(source.GetType(), GetEmptyMethod);
            var empty = emptyMethod.Invoke(null, null);
            IntersectWith(source, empty);
        }

        private static MethodInfo GetUnionWithMethod(Type type)
        {
            var itemType = type.GetItemType();
            return type.GetMethod("UnionWith", new[] { typeof(IEnumerable<>).MakeGenericType(itemType) });
        }

        private static MethodInfo GetIntersectWithMethod(Type type)
        {
            var itemType = type.GetItemType();
            return type.GetMethod("IntersectWith", new[] { typeof(IEnumerable<>).MakeGenericType(itemType) });
        }

        private static MethodInfo GetEmptyMethod(Type type)
        {
            var itemType = type.GetItemType();
            var emptyMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Empty), BindingFlags.Static | BindingFlags.Public)
                                                .MakeGenericMethod(itemType);
            return emptyMethod;
        }
    }
}