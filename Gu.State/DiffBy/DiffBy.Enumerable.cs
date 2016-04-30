namespace Gu.State
{
    using System;

    public static partial class DiffBy
    {
        private static class Enumerable
        {
            internal static void AddItemDiffs<TSettings>(
                object x,
                object y,
                TSettings settings,
                DiffBuilder collectionBuilder,
                Action<object, object, object, TSettings, DiffBuilder> itemDiff)
                where TSettings : IMemberSettings
            {
                if (!Is.Enumerable(x, y))
                {
                    return;
                }

                IDiffBy comparer;
                if (ListDiffBy.TryGetOrCreate(x, y, out comparer) ||
                    ReadonlyListDiffBy.TryGetOrCreate(x, y, out comparer) ||
                    ArrayDiffBy.TryGetOrCreate(x, y, out comparer) ||
                    DictionaryDiffBy.TryGetOrCreate(x, y, out comparer) ||
                    ReadOnlyDictionaryDiffBy.TryGetOrCreate(x, y, out comparer) ||
                    SetDiffBy.TryGetOrCreate(x, y, out comparer) ||
                    EnumerableDiffBy.TryGetOrCreate(x, y, out comparer))
                {
                    comparer.AddDiffs(x, y, settings, collectionBuilder, itemDiff);
                    return;
                }

                throw Throw.ShouldNeverGetHereException("All enumarebles must be checked here");
            }
        }
    }
}
