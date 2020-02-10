namespace Gu.State
{
    /// <summary>
    /// Info about a graph change.
    /// </summary>
    /// <typeparam name="T">The type of node.</typeparam>
    public abstract class GraphChangeEventArgs<T> : TrackerChangedEventArgs<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphChangeEventArgs{T}"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="previous">The previous change.</param>
        protected GraphChangeEventArgs(T node, TrackerChangedEventArgs<T> previous = null)
             : base(node, previous)
        {
        }
    }
}
