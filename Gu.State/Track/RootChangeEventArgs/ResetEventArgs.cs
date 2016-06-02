namespace Gu.State
{
    using System.Collections;

    /// <summary>This is raised when a notifying collection signals reset.</summary>
    public struct ResetEventArgs : IRootChangeEventArgs
    {
        internal ResetEventArgs(IList source)
        {
            this.Source = source;
        }

        /// <summary>Gets the collection that changed.</summary>
        public IList Source { get; }

        /// <inheritdoc />
        object IRootChangeEventArgs.Source => this.Source;
    }
}