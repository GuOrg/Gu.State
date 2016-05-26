namespace Gu.State
{
    public class RootChangeEventArgs<T> : TrackerChangedEventArgs<T>
    {
        public RootChangeEventArgs(T node, IRootChangeEventArgs eventArgs)
            : base(node, null)
        {
            this.EventArgs = eventArgs;
        }

        public IRootChangeEventArgs EventArgs { get; }
    }
}