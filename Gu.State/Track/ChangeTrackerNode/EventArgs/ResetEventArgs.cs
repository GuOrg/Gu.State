namespace Gu.State
{
    using System.Collections;

    internal struct ResetEventArgs
    {
        private static readonly IList Empty = new object[0];

        public ResetEventArgs(IList oldItems, IList newItems)
        {
            this.OldItems = oldItems ?? Empty;
            this.NewItems = newItems ?? Empty;
        }

        public IList OldItems { get; }

        public IList NewItems { get; }
    }
}