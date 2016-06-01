namespace Gu.State
{
    using System.Collections;

    /// <summary>This is raised when an element was added to a notifying collection.</summary>
    public struct AddEventArgs : IRootChangeEventArgs
    {
        internal AddEventArgs(IList source, int index)
        {
            this.Index = index;
            this.Source = source;
        }

        /// <summary>Gets the collection that changed.</summary>
        public IList Source { get; private set; }

        /// <inheritdoc />
        object IRootChangeEventArgs.Source => this.Source;

        /// <summary>Gets the index at which an element was added. </summary>
        public int Index { get; }
    }
}