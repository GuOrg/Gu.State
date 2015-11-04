namespace Gu.ChangeTracking
{
    /// <summary>
    /// Tracks 
    /// </summary>
    public interface IValueTracker : ITracker
    {
        /// <summary>
        /// Gets the current value
        /// </summary>
        object Value { get; }
    }
}