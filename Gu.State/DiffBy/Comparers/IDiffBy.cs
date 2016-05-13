namespace Gu.State
{
    internal interface IDiffBy
    {
        void AddDiffs(
            DiffBuilder collectionBuilder,
            object x,
            object y,
            IMemberSettings settings);
    }
}