namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal static class Set
    {
        private static ConcurrentDictionary<Type, MethodInfo> EmptyMethods = new ConcurrentDictionary<Type, MethodInfo>();
        private static ConcurrentDictionary<Type, MethodInfo> SetEqualsMethods = new ConcurrentDictionary<Type, MethodInfo>();
        private static ConcurrentDictionary<Type, MethodInfo> UnionWithMethods = new ConcurrentDictionary<Type, MethodInfo>();
        private static ConcurrentDictionary<Type, MethodInfo> IntersectWithMethods = new ConcurrentDictionary<Type, MethodInfo>();

        internal static bool Add(object set, object value)
        {
            var addMethod = set.GetType().GetMethod("Add", new[] { set.GetType().GetItemType() });
            return (bool)addMethod.Invoke(set, new[] { value });
        }

        internal static bool TryOrderByHashCode(object set, out IEnumerable<object> ordered)
        {
            var comparer = set.GetType()
                              .GetProperty("Comparer", Constants.DefaultPropertyBindingFlags)
                              ?.GetValue(set);

            if (comparer == null)
            {
                ordered = ((IEnumerable)set).Cast<object>()
                                            .OrderBy(i => i.GetHashCode());
            }
            else
            {
                var getHashcodeMethod = comparer.GetType()
                                                .GetMethod(nameof(GetHashCode), new[] { set.GetType().GetItemType() });
                ordered = ((IEnumerable)set).Cast<object>()
                                            .OrderBy(i => (int)getHashcodeMethod.Invoke(comparer, new[] { i }));
            }

            return !collision;
        }

        internal new static bool Equals(object first, object other)
        {
            var methodInfo = SetEqualsMethods.GetOrAdd(first.GetType(), GetSetEqualsMethod);
            return (bool)methodInfo.Invoke(first, new[] { other });
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

        private static MethodInfo GetSetEqualsMethod(Type type)
        {
            var methodInfo = type.GetMethod("SetEquals", BindingFlags.Public | BindingFlags.Instance);
            Debug.Assert(methodInfo != null, "setEqualsMethod == null");
            return methodInfo;
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

        public static IEnumerable<PaddedPairs.Pair<object>> Pairs(object source, object target)
        {
            var se = ElementsOrderedByHashCode((IEnumerable)source);
            var te = ElementsOrderedByHashCode((IEnumerable)target);
            var targetEnumerator = te.GetEnumerator();
            foreach (var o in se)
            {

            }
            throw new NotImplementedException("message");

        }
    }
}