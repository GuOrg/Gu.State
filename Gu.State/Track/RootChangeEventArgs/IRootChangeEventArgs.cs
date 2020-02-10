namespace Gu.State
{
    /// <summary>
    /// Information about a graph root change.
    /// </summary>
    public interface IRootChangeEventArgs
    {
        /// <summary>Gets the source instance that changed.</summary>
        object Source { get; }
    }
}
