namespace Gu.ChangeTracking
{
    /// <summary>
    /// Enum telling the change tracker how to handle an instance
    /// </summary>
    public enum TrackAs
    {
        /// <summary>
        /// No value specified. This is illegal
        /// </summary>
        Unknown,
        
        /// <summary>
        /// The value is treated as immutable and the tracker does not track changes to sub properties.
        /// Useful for types like <see cref="System.DateTime"/>
        /// </summary>
        Immutable,

        /// <summary>
        /// The tracker does not track changes to subproperties or items.
        /// It is your responsibility to track these.
        /// </summary>
        Explicit,

        /// <summary>
        /// The tracker does not track changes to subproperties or items.
        /// It is your responsibility to track these.
        /// </summary>
        Ignored
    }
}