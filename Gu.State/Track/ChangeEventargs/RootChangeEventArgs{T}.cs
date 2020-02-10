namespace Gu.State
{
    /// <summary>The source of a change.</summary>
    /// <typeparam name="T">The type of tracker.</typeparam>
    public class RootChangeEventArgs<T> : TrackerChangedEventArgs<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootChangeEventArgs{T}"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="eventArgs">The change info.</param>
        public RootChangeEventArgs(T node, IRootChangeEventArgs eventArgs)
            : base(node, null)
        {
            this.EventArgs = eventArgs;
        }

        /// <summary>Gets the root change args.</summary>
        public IRootChangeEventArgs EventArgs { get; }
    }
}
