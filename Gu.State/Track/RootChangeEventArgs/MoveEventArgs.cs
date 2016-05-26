namespace Gu.State
{
    /// <summary>This is raised when an element is moved in a notifying collection.</summary>
    public struct MoveEventArgs : IRootChangeEventArgs
    {
        internal MoveEventArgs(int fromIndex, int toIndex)
        {
            this.FromIndex = fromIndex;
            this.ToIndex = toIndex;
        }

        /// <summary>Gets the index at which an element was moved from. </summary>
        public int FromIndex { get; }

        /// <summary>Gets the index at which an element was moved to. </summary>
        public int ToIndex { get; }
    }
}