namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class NullableEqualByComparer
    {
        internal static bool TryGet(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (type.IsNullable())
            {
                comparer = (EqualByComparer)typeof(Comparer<>).MakeGenericType(type.GenericTypeArguments)
                                                                      .GetField(nameof(Comparer<int>.Default), BindingFlags.Public | BindingFlags.Static)
                                                                      .GetValue(null);
                return true;
            }

            comparer = null;
            return false;
        }

        internal class Comparer<T> : EqualByComparer<T?>
            where T : struct
        {
            public static readonly Comparer<T> Default = new Comparer<T>();
            private static readonly bool IsEquatable = typeof(T).Equatable();

            private Comparer()
            {
            }

            internal override bool TryGetError(MemberSettings settings, out Error error)
            {
                if (settings.GetEqualByComparer(typeof(T)) is ErrorEqualByComparer errorEqualByComparer)
                {
                    error = errorEqualByComparer.Error;
                    return true;
                }

                error = null;
                return false;
            }

            internal override bool Equals(T? x, T? y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
            {
                if (IsEquatable)
                {
                    return Nullable.Equals(x, y);
                }

                return settings.GetEqualByComparer(typeof(T)).Equals(x.Value, y.Value, settings, referencePairs);
            }
        }
    }
}
