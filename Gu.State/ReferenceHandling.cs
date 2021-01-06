namespace Gu.State
{
    /// <summary>
    /// Provides instructions for how sub-members in graphs are handled.
    /// </summary>
    public enum ReferenceHandling
    {
        /// <summary>
        /// Throws for collections and members that are complex types.
        /// </summary>
        Throw,

        /// <summary>
        /// Compares using object.ReferenceEquals(x, y)
        /// Copies values by reference.
        /// </summary>
        References,

        /// <summary>
        /// Compares by walking the graph comparing sub-members of IEquatable types
        /// Copies by walking the graph copying immutable sub-member values.
        /// </summary>
        Structural,
    }
}
