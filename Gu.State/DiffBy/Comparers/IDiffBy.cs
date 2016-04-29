namespace Gu.State
{
    using System;

    interface IDiffBy
    {
        void AddDiffs<TSettings>(
            object x,
            object y,
            TSettings settings,
            DiffBuilder collectionBuilder,
            Action<object, object, object, TSettings, DiffBuilder> itemDiff)
            where TSettings : IMemberSettings;
    }
}