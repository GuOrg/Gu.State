namespace Gu.State
{
    using System;

    /// <summary>
    /// A custom copy implementation.
    /// </summary>
    /// <typeparam name="T">The type to copy.</typeparam>
    public class CustomCopy<T> : CustomCopy
    {
        private readonly Func<T, T, T> copyValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomCopy{T}"/> class.
        /// </summary>
        /// <typeparam name="T">The type to copy.</typeparam>
        /// <param name="copyValue">The implementation.</param>
        public CustomCopy(Func<T, T, T> copyValue)
        {
            this.copyValue = copyValue;
        }

        /// <inheritdoc/>
        public override object Copy(object source, object target) => this.Copy((T)source, (T)target);

        /// <summary>
        /// Copy from <paramref name="source"/> to <paramref name="target"/>.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>The result of the copy.</returns>
        public T Copy(T source, T target) => this.copyValue(source, target);
    }
}
