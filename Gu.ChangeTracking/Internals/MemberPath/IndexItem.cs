namespace Gu.ChangeTracking
{
    internal class IndexItem : PathItem
    {
        public IndexItem(int? index)
        {
            this.Index = index;
        }

        public int? Index { get; }
    }
}