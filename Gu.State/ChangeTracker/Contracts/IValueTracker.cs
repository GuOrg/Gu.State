namespace Gu.State
{
    /// <summary>
    /// Tracks
    /// </summary>
    public interface IValueTracker : IChangeTracker
    {
        /// <summary>
        /// Gets the current value
        /// </summary>
        object Value { get; }
    }
}