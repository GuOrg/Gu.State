namespace Gu.ChangeTracking
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