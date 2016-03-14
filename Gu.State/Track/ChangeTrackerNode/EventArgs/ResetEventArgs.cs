namespace Gu.State
{
    using System.Collections;

    internal struct ResetEventArgs
    {
        public ResetEventArgs(IList oldItems, IList newItems)
        {
            this.OldItems = oldItems;
            this.NewItems = newItems;
        }

        public IList OldItems { get; }

        public IList NewItems { get; }
    }
}