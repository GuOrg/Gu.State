﻿namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal static class DictionaryExt
    {
        internal static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
            where TValue : IDisposable
        {
            if (dictionary.TryGetValue(key, out var old))
            {
                old?.Dispose();
            }

            dictionary[key] = value;
        }

        internal static void Move<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey from, TKey to)
    where TValue : IDisposable
        {
            if (dictionary.TryGetValue(from, out var value))
            {
                dictionary.Remove(from);
                dictionary[to] = value;
            }
        }

        internal static void TryRemoveAndDispose<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
            where TValue : IDisposable
        {
            if (dictionary.TryGetValue(key, out var old))
            {
                old?.Dispose();
            }

            dictionary.Remove(key);
        }
    }
}
