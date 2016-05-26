namespace Gu.State
{
    using System.Collections;

    public struct ResetEventArgs : IRootChangeEventArgs
    {
        private static readonly IList Empty = new object[0];

        public ResetEventArgs(IList oldItems, IList newItems)
        {
            this.OldItems = oldItems ?? Empty;
            this.NewItems = newItems ?? Empty;
        }

        internal IList OldItems { get; }

        internal IList NewItems { get; }
    }
}