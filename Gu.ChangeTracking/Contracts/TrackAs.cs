namespace Gu.ChangeTracking
{
    using System;

    /// <summary>
    /// Enum telling the change tracker how to handle an instance
    /// </summary>
    [Flags]
    public enum TrackAs
    {
        None = 0,
        /// <summary>
        /// The value is treated as immutable and the tracker does not track changes to sub properties.
        /// Useful for types like <see cref="System.DateTime"/>
        /// </summary>
        Immutable = 1 << 0,

        /// <summary>
        /// The tracker does not track changes to subproperties or items.
        /// It is your responsibility to track these.
        /// </summary>
        Ignored = 1 << 1,
    }
}