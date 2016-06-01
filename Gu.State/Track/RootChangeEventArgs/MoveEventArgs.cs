namespace Gu.State
{
    using System.Collections;

    /// <summary>This is raised when an element is moved in a notifying collection.</summary>
    public struct MoveEventArgs : IRootChangeEventArgs
    {
        internal MoveEventArgs(IList source, int fromIndex, int toIndex)
        {
            this.FromIndex = fromIndex;
            this.ToIndex = toIndex;
            this.Source = source;
        }

        /// <summary>Gets the collection that changed.</summary>
        public IList Source { get; private set; }

        /// <inheritdoc />
        object IRootChangeEventArgs.Source => this.Source;

        /// <summary>Gets the index at which an element was moved from. </summary>
        public int FromIndex { get; }

        /// <summary>Gets the index at which an element was moved to. </summary>
        public int ToIndex { get; }
    }
}