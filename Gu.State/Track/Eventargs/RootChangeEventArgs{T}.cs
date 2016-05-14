namespace Gu.State
{
    internal class RootChangeEventArgs<T> : TrackerChangedEventArgs<T>
    {
        internal readonly IRootChangeEventArgs EventArgs;

        public RootChangeEventArgs(T node, IRootChangeEventArgs eventArgs)
            : base(node, null)
        {
            this.EventArgs = eventArgs;
        }
    }
}