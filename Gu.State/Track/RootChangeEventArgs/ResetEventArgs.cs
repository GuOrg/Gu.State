namespace Gu.State
{
    using System.Collections;

    /// <summary>This is raised when a notifying collection signals reset.</summary>
    public struct ResetEventArgs : IRootChangeEventArgs
    {
        private static readonly IList Empty = new object[0];

        internal ResetEventArgs(IList oldItems, IList newItems)
        {
            this.OldItems = oldItems ?? Empty;
            this.NewItems = newItems ?? Empty;
        }

        internal IList OldItems { get; }

        internal IList NewItems { get; }
    }
}