namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal static class IReadOnlyDictionaryEqualByComparer
    {
        internal static bool TryCreate(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (type.Implements(typeof(IReadOnlyDictionary<,>)))
            {
                var dictionaryType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>)
                    ? type
                    : type.GetInterface("IReadOnlyDictionary`2");
                comparer = (EqualByComparer)Activator.CreateInstance(
                    typeof(Comparer<,,>).MakeGenericType(type, dictionaryType.GenericTypeArguments[0], dictionaryType.GenericTypeArguments[1]),
                    settings.GetEqualByComparerOrDeferred(dictionaryType.GenericTypeArguments[0]),
                    settings.GetEqualByComparerOrDeferred(dictionaryType.GenericTypeArguments[1]));
                return true;
            }

            comparer = null;
            return false;
        }

        private class Comparer<TMap, TKey, TValue> : EqualByComparer<IReadOnlyDictionary<TKey, TValue>>
        {
            private readonly ISetEqualByComparer.EqualByComparer<IEnumerable<TKey>, TKey> keysComparer;
            private readonly EqualByComparer valueComparer;

            public Comparer(EqualByComparer keyComparer, EqualByComparer valueComparer)
            {
                this.keysComparer = new ISetEqualByComparer.EqualByComparer<IEnumerable<TKey>, TKey>(keyComparer);
                this.valueComparer = valueComparer;
            }

            internal override bool CanHaveReferenceLoops => this.keysComparer.CanHaveReferenceLoops ||
                                                            this.valueComparer.CanHaveReferenceLoops;

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

                return this.keysComparer.Equals(x.Keys, y.Keys, settings, referencePairs) &&
                       ValuesEquals(x, y, this.valueComparer, settings, referencePairs);
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
        }
    }
}
