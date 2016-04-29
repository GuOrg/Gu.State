namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal static partial class Set
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo> EmptyMethods = new ConcurrentDictionary<Type, MethodInfo>();
        private static readonly ConcurrentDictionary<Type, MethodInfo> UnionWithMethods = new ConcurrentDictionary<Type, MethodInfo>();
        private static readonly ConcurrentDictionary<Type, MethodInfo> IntersectWithMethods = new ConcurrentDictionary<Type, MethodInfo>();
        private static readonly ConcurrentDictionary<Type, ConstructorInfo> SortedCtors = new ConcurrentDictionary<Type, ConstructorInfo>();

        internal static bool Add(object set, object value)
        {
            var addMethod = set.GetType().GetMethod("Add", new[] { set.GetType().GetItemType() });
            return (bool)addMethod.Invoke(set, new[] { value });
        }

        internal static ISortedByHashCode ItemsOrderByHashCode(object set)
        {
            var setType = set.GetType();
            var comparer = setType.GetProperty("Comparer", Constants.DefaultPropertyBindingFlags)
                                 ?.GetValue(set);
            var ctor = SortedCtors.GetOrAdd(setType, GetSortedCtor);
            return (ISortedByHashCode)ctor.Invoke(new[] { set, comparer });
        }

        internal static void UnionWith(object set, object otherSet)
        {
            var methodInfo = UnionWithMethods.GetOrAdd(set.GetType(), GetUnionWithMethod);
            methodInfo.Invoke(set, new[] { otherSet });
        }

        internal static void IntersectWith(object set, object otherSet)
        {
            var methodInfo = IntersectWithMethods.GetOrAdd(set.GetType(), GetIntersectWithMethod);
            methodInfo.Invoke(set, new[] { otherSet });
        }

        internal static void Clear(object source)
        {
            var methodInfo = EmptyMethods.GetOrAdd(source.GetType(), GetEmptyMethod);
            var empty = methodInfo.Invoke(null, null);
            IntersectWith(source, empty);
        }

        internal static IEnumerable<PaddedPairs.Pair<object>> Pairs(object source, object target)
        {
            var se = ItemsOrderByHashCode(source);
            var te = ItemsOrderByHashCode(target);
            foreach (var sv in se)
            {
                var indices = te.MatchingHashIndices(sv);
                if (indices.IsNone)
                {
                    yield return new PaddedPairs.Pair<object>(sv, PaddedPairs.MissingItem);
                    continue;
                }

                yield return new PaddedPairs.Pair<object>(sv, te[indices.First]);
                te.RemoveAt(indices.First);
            }

            foreach (var tv in te)
            {
                yield return new PaddedPairs.Pair<object>(PaddedPairs.MissingItem, tv);
            }
        }

        private static ConstructorInfo GetSortedCtor(Type type)
        {
            var itemType = type.GetItemType();
            var types = new[] { typeof(IEnumerable<>).MakeGenericType(itemType), typeof(IEqualityComparer<>).MakeGenericType(itemType) };
            var constructorInfo = typeof(SortedByHashCode<>).MakeGenericType(itemType)
                                                            .GetConstructor(types);
            Debug.Assert(constructorInfo != null, "constructorInfo == null");
            return constructorInfo;
        }

        private static MethodInfo GetUnionWithMethod(Type type)
        {
            var itemType = type.GetItemType();
            var methodInfo = type.GetMethod("UnionWith", new[] { typeof(IEnumerable<>).MakeGenericType(itemType) });
            Debug.Assert(methodInfo != null, "methodInfo == null");
            return methodInfo;
        }

        private static MethodInfo GetIntersectWithMethod(Type type)
        {
            var itemType = type.GetItemType();
            var methodInfo = type.GetMethod("IntersectWith", new[] { typeof(IEnumerable<>).MakeGenericType(itemType) });
            Debug.Assert(methodInfo != null, "methodInfo == null");
            return methodInfo;
        }

        private static MethodInfo GetEmptyMethod(Type type)
        {
            var itemType = type.GetItemType();
            var methodInfo = typeof(Enumerable).GetMethod(nameof(Enumerable.Empty), BindingFlags.Static | BindingFlags.Public)
                                                .MakeGenericMethod(itemType);
            Debug.Assert(methodInfo != null, "methodInfo == null");
            return methodInfo;
        }
    }
}