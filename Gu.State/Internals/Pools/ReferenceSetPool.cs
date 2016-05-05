namespace Gu.State
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    internal static class ReferenceSetPool
    {
        private static readonly ConcurrentQueue<HashSet<object>> Pool = new ConcurrentQueue<HashSet<object>>();

        internal static Disposer<HashSet<object>> Borrow()
        {
            HashSet<object> set;
            if (Pool.TryDequeue(out set))
            {
                return Disposer.Create(set, Return);
            }
            else
            {
                return Disposer.Create(new HashSet<object>(ReferenceComparer.Default), Return);
            }
        }

        private static void Return(HashSet<object> set)
        {
            set.Clear();
            Pool.Enqueue(set);
        }
    }
}