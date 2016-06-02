namespace Gu.State
{
    using System.Collections;

    /// <summary>This is raised when an element was replaced in a notifying collection.</summary>
    public struct ReplaceEventArgs : IRootChangeEventArgs
    {
        internal ReplaceEventArgs(IList source, int index)
        {
            this.Index = index;
            this.Source = source;
        }

        /// <summary>Gets the collection that changed.</summary>
        public IList Source { get; }

        /// <inheritdoc />
        object IRootChangeEventArgs.Source => this.Source;

        /// <summary>Gets the index at which an element was replaced. </summary>
        public int Index { get; }
    }
}