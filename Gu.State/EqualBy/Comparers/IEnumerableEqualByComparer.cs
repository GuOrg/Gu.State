namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class IEnumerableEqualByComparer
    {
        internal static bool TryGet(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (type.Implements(typeof(IEnumerable<>)))
            {
                comparer = (EqualByComparer)typeof(Comparer<>).MakeGenericType(type.GetItemType())
                                                              .GetField(nameof(Comparer<int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                              .GetValue(null);
                return true;
            }

            comparer = null;
            return false;
        }

        private class Comparer<T> : CollectionEqualByComparer<IEnumerable<T>,T>
        {
            /// <summary>The default instance.</summary>
            public static readonly Comparer<T> Default = new Comparer<T>();

            private Comparer()
            {
            }

            internal override bool Equals(IEnumerable<T> x, IEnumerable<T> y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                var comparer = settings.GetEqualByComparer(typeof(T));
                using (var xe = x.GetEnumerator())
                {
                    using (var ye = y.GetEnumerator())
                    {
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
    }
}