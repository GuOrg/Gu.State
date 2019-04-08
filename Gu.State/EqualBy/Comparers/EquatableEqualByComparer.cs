namespace Gu.State
{
    using System;
    using System.Reflection;

    internal static class EquatableEqualByComparer
    {
        internal static bool TryGet(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (settings.IsEquatable(type) &&
                type.Name != "ImmutableArray`1")
            {
                comparer = (EqualByComparer)typeof(ExplicitEqualByComparer<>).MakeGenericType(type)
                                                              .GetField(nameof(ExplicitEqualByComparer<int>.Default), BindingFlags.Public | BindingFlags.Static)
                                                              .GetValue(null);
                return true;
            }

            comparer = null;
            return false;
        }
    }
}
