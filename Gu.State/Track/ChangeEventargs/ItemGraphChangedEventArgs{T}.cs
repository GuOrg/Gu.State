namespace Gu.State
{
    public class ItemGraphChangedEventArgs<T> : GraphChangeEventArgs<T>
    {
        public ItemGraphChangedEventArgs(T node, int index, TrackerChangedEventArgs<T> previous = null)
            : base(node, previous)
        {
            this.Index = index;
        }

        public int Index { get; }
    }
}