namespace Gu.State
{
    using System;

    /// <summary>
    /// A custom copy implementation.
    /// </summary>
    public abstract class CustomCopy
    {
        /// <summary>
        /// Create a <see cref="CustomCopy{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type to copy.</typeparam>
        /// <param name="copyMethod">The implementation.</param>
        /// <returns>A <see cref="CustomCopy{T}"/>.</returns>
        public static CustomCopy Create<T>(Func<T, T, T> copyMethod)
        {
            return new CustomCopy<T>(copyMethod);
        }

        /// <summary>
        /// Copy from <paramref name="source"/> to <paramref name="target"/>.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>The result of the copy.</returns>
        public abstract object Copy(object source, object target);
    }
}
