namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class NullableEqualByComparer
    {
        internal static bool TryGet(Type type, out EqualByComparer comparer)
        {
            if (type.IsNullable())
            {
                comparer = (EqualByComparer)typeof(Comparer<>).MakeGenericType(type.GenericTypeArguments)
                                                                      .GetField(nameof(Comparer<int>.Default), BindingFlags.NonPublic | BindingFlags.Static)
                                                                      .GetValue(null);
                return true;
            }

            comparer = null;
            return false;
        }

        internal sealed class Comparer<T> : EqualByComparer<T?>
            where T : struct
        {
            internal static readonly Comparer<T> Default = new();

            private static readonly bool IsEquatable = typeof(T).Equatable();

            private Comparer()
            {
            }

            internal override bool CanHaveReferenceLoops => !IsEquatable;

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
