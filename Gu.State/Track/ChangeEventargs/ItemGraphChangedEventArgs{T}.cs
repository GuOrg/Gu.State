namespace Gu.State
{
    /// <summary>This is raised when a change bubbled up for an item in a collection.</summary>
    /// <typeparam name="T">The type of tracker.</typeparam>
    public class ItemGraphChangedEventArgs<T> : GraphChangeEventArgs<T>
    {
        internal ItemGraphChangedEventArgs(T node, int index, TrackerChangedEventArgs<T> previous = null)
            : base(node, previous)
        {
            this.Index = index;
        }

        /// <summary>Gets the index of the item for which a change happened.</summary>
        public int Index { get; }
    }
}