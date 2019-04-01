namespace Gu.State
{
    using System.Collections.Generic;

    /// <summary>A wrapper for a generic comparer.</summary>
    public abstract class CastingComparer
    {
        /// <summary>
        /// Create a <see cref="CastingComparer"/> for <paramref name="comparer"/>.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="comparer">The comparer.</param>
        /// <returns>A wrapper for <paramref name="comparer"/>.</returns>
        public static CastingComparer Create<T>(IEqualityComparer<T> comparer)
        {
            return new CastingComparer<T>(comparer);
        }

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        /// <param name="x">
        ///     The first object to compare.
        /// </param>
        /// <param name="y">
        ///     The second object to compare.
        /// </param>
        public new abstract bool Equals(object x, object y);

        /// <summary>
        ///     Returns a hash code for the specified object.
        /// </summary>
        /// <returns>A hash code for the specified object.</returns>
        /// <param name="obj">
        ///     The <see cref="object" /> for which a hash code is to be returned.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is null.
        /// </exception>
        public abstract int GetHashCode(object obj);
    }
}