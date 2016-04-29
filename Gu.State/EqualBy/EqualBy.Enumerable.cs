namespace Gu.State
{
    using System;
    using System.Diagnostics;

    public static partial class EqualBy
    {
        private static bool EnumerableEquals<TSetting>(
            object x,
            object y,
            Func<object, object, TSetting, ReferencePairCollection, bool> compareItem,
            TSetting settings,
            ReferencePairCollection referencePairs)
            where TSetting : class, IMemberSettings
        {
            EqualByComparer comparer;
            if (ListEqualByComparer.TryGetOrCreate(x, y, out comparer) ||
                ReadOnlyListEqualByComparer.TryGetOrCreate(x, y, out comparer) ||
                SetEqualByComparer.TryGetOrCreate(x, y, out comparer) ||
                ArrayEqualByComparer.TryGetOrCreate(x, y, out comparer) ||
                DictionaryEqualByComparer.TryGetOrCreate(x, y, out comparer) ||
                ReadOnlyDictionaryEqualByComparer.TryGetOrCreate(x, y, out comparer) ||
                EnumerableEqualByComparer.TryGetOrCreate(x, y, out comparer))
            {
                return comparer.Equals(x, y, compareItem, settings, referencePairs);
            }

            throw Throw.ShouldNeverGetHereException($"Could not compare enumerables of type {x.GetType().PrettyName()}");
        }
    }
}
