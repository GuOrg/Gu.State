namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class IReadOnlyListEqualByComparer
    {
        internal static bool TryGet(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (type.Implements(typeof(IReadOnlyList<>)))
            {
                var itemType = type.GetItemType();

                // resolve comparer so we throw as early as possible if there are errors.
                _ = settings.GetEqualByComparer(itemType, checkReferenceHandling: true);

                if (type.IsArray)
                {
                    comparer = (EqualByComparer)typeof(ArrayComparer<>).MakeGenericType(itemType)
                                                                  .GetField(nameof(ArrayComparer<int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                                  .GetValue(null);
                }
                else if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    comparer = (EqualByComparer)typeof(ListComparer<>).MakeGenericType(itemType)
                                                                      .GetField(nameof(ListComparer<int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                                      .GetValue(null);
                }
                else
                {
                    comparer = (EqualByComparer)typeof(Comparer<>).MakeGenericType(itemType)
                                                                  .GetField(nameof(Comparer<int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                                  .GetValue(null);
                }

                return true;
            }

            comparer = null;
            return false;
        }

        private class Comparer<T> : EqualByComparer
        {
            public static Comparer<T> Default = new Comparer<T>();

            private Comparer()
            {
            }

            public override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                return TryGetEitherNullEquals(x, y, out var result)
                    ? result
                    : this.Equals((IReadOnlyList<T>)x, (IReadOnlyList<T>)y, settings, referencePairs);
            }

            private bool Equals(IReadOnlyList<T> x, IReadOnlyList<T> y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                if (x.Count != y.Count)
                {
                    return false;
                }

                var comparer = settings.GetEqualByComparer(typeof(T), checkReferenceHandling: true);
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

        private class ListComparer<T> : EqualByComparer
        {
            public static ListComparer<T> Default = new ListComparer<T>();

            private ListComparer()
            {
            }

            public override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                return TryGetEitherNullEquals(x, y, out var result)
                    ? result
                    : this.Equals((List<T>)x, (List<T>)y, settings, referencePairs);
            }

            private bool Equals(List<T> x, List<T> y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                if (x.Count != y.Count)
                {
                    return false;
                }

                var comparer = settings.GetEqualByComparer(typeof(T), checkReferenceHandling: true);
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

        private class ArrayComparer<T> : EqualByComparer
        {
            public static ArrayComparer<T> Default = new ArrayComparer<T>();

            private ArrayComparer()
            {
            }

            public override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                return TryGetEitherNullEquals(x, y, out var result)
                    ? result
                    : this.Equals((T[])x, (T[])y, settings, referencePairs);
            }

            private bool Equals(T[] x, T[] y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                if (x.Length != y.Length)
                {
                    return false;
                }

                var comparer = settings.GetEqualByComparer(typeof(T), checkReferenceHandling: true);
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