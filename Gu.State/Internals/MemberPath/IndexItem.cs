namespace Gu.State
{
    internal class IndexItem : PathItem
    {
        internal IndexItem(int? index)
        {
            this.Index = index;
        }

        internal int? Index { get; }
    }
}