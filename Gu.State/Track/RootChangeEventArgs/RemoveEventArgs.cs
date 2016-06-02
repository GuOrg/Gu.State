namespace Gu.State
{
    using System.Collections;

    /// <summary>This is raised when an element is removed from a notifying collection.</summary>
    public struct RemoveEventArgs : IRootChangeEventArgs
    {
        internal RemoveEventArgs(IList source, int index)
        {
            this.Index = index;
            this.Source = source;
        }

        /// <summary>Gets the collection that changed.</summary>
        public IList Source { get; }

        /// <inheritdoc />
        object IRootChangeEventArgs.Source => this.Source;

        /// <summary>Gets the index at which an element was removed. </summary>
        public int Index { get; }
    }
}