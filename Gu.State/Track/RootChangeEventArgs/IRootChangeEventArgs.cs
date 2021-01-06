namespace Gu.State
{
    /// <summary>
    /// Information about a graph root change.
    /// </summary>
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public interface IRootChangeEventArgs
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        /// <summary>Gets the source instance that changed.</summary>
        object Source { get; }
    }
}
