namespace Gu.State
{
    internal class ItemGraphChangedEventArgs<T> : GraphChangeEventArgs<T>
    {
        internal readonly int Index;

        public ItemGraphChangedEventArgs(T node, int index, TrackerChangedEventArgs<T> previous = null)
            : base(node, previous)
        {
            this.Index = index;
        }
    }
}