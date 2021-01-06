namespace Gu.State
{
    /// <summary>
    /// Interface for harvesting differences.
    /// </summary>
    internal interface IDiffBy
    {
        /// <summary>
        /// Add differences between <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// <param name="builder">The <see cref="DiffBuilder"/>.</param>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <param name="settings">The <see cref="MemberSettings"/>.</param>
        void AddDiffs(
            DiffBuilder builder,
            object x,
            object y,
            MemberSettings settings);
    }
}
