namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    internal static class IReadOnlyDictionaryEqualByComparer
    {
        private static readonly ConcurrentDictionary<Type, EqualByComparer> Cache = new ConcurrentDictionary<Type, EqualByComparer>();

        internal static bool TryCreate(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (type.Implements(typeof(IReadOnlyDictionary<,>)))
            {
                var dictionaryType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>)
                    ? type
                    : type.GetInterface("IReadOnlyDictionary`2");
                comparer = (EqualByComparer)Activator.CreateInstance(typeof(Comparer<,,>).MakeGenericType(type, dictionaryType.GenericTypeArguments[0], dictionaryType.GenericTypeArguments[1]));
                return true;
            }

            comparer = null;
            return false;
        }

        private class Comparer<TMap, TKey, TValue> : EqualByComparer<IReadOnlyDictionary<TKey, TValue>>
        {
            private readonly ISetEqualByComparer.EqualByComparer<IEnumerable<TKey>, TKey> keysEqualByComparer = new ISetEqualByComparer.EqualByComparer<IEnumerable<TKey>, TKey>();
            private EqualByComparer lazyValueComparer;

            internal override bool TryGetError(MemberSettings settings, out Error error)
            {
                if (CollectionEqualByComparer<TMap, TKey>.TryGetItemError(settings, out var keyError))
                {
                    error = keyError;
                    return true;
                }

                if (CollectionEqualByComparer<TMap, TValue>.TryGetItemError(settings, out var valueError))
                {
                    error = new TypeErrors(typeof(TMap), valueError);
                    return true;
                }

                error = null;
                return false;
            }

            internal override bool Equals(IReadOnlyDictionary<TKey, TValue> x, IReadOnlyDictionary<TKey, TValue> y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
            {
                if (x.Count != y.Count)
                {
                    return false;
                }

                return this.keysEqualByComparer.Equals(x.Keys, y.Keys, settings, referencePairs) &&
                       ValuesEquals(x, y, this.ValueComparer(settings), settings, referencePairs);
            }

            private static bool ValuesEquals(IReadOnlyDictionary<TKey, TValue> x, IReadOnlyDictionary<TKey, TValue> y, EqualByComparer valueComparer, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
            {
                foreach (var key in x.Keys)
                {
                    if (!y.TryGetValue(key, out var yv))
                    {
                        return false;
                    }

                    if (!valueComparer.Equals(x[key], yv, settings, referencePairs))
                    {
                        return false;
                    }
                }

                return true;
            }

            private EqualByComparer ValueComparer(MemberSettings settings)
            {
                if (this.lazyValueComparer is null)
                {
                    this.lazyValueComparer = typeof(TValue).IsSealed
                        ? settings.GetEqualByComparer(typeof(TValue))
                        : new LazyEqualByComparer<TValue>();
                }

                return this.lazyValueComparer;
            }
        }
    }
}