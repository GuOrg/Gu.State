namespace Gu.State
{
    using System;
    using System.Reflection;

    internal static class ArrayEqualByComparer
    {
        internal static bool TryGet(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (type.IsArray)
            {
                switch (type.GetArrayRank())
                {
                    case 1:
                        return IReadOnlyListEqualByComparer.TryGet(type, settings, out comparer);
                    case 2:
                        comparer = (EqualByComparer)typeof(Comparer2D<>).MakeGenericType(type.GetItemType())
                                                                           .GetField(nameof(Comparer2D<int>.Default), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                                                                           .GetValue(null);
                        return true;
                    default:
                        comparer = Comparer.Default;
                        return true;
                }
            }

            comparer = null;
            return false;
        }

        private class Comparer2D<T> : EqualByComparer<T[,]>
        {
            /// <summary>The default instance.</summary>
            public static readonly Comparer2D<T> Default = new Comparer2D<T>();

            private Comparer2D()
            {
            }

            public override bool Equals(T[,] xs, T[,] ys, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                if (!Is.SameSize(xs, ys))
                {
                    return false;
                }

                var comparer = settings.GetEqualByComparer(typeof(T));

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

        private class Comparer : EqualByComparer
        {
            /// <summary>The default instance.</summary>
            public static readonly Comparer Default = new Comparer();

            private Comparer()
            {
            }

            public override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                if (TryGetEitherNullEquals(x, y, out var result))
                {
                    return result;
                }

                return Equals((Array)x, (Array)y, settings, referencePairs);
            }

            private static bool Equals(Array x, Array y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                if (!Is.SameSize(x, y))
                {
                    return false;
                }

                var comparer = settings.GetEqualByComparer(x.GetType().GetItemType());
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
