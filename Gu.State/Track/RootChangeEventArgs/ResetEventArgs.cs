namespace Gu.State
{
    using System;
    using System.Collections;

    /// <summary>This is raised when a notifying collection signals reset.</summary>
    public readonly struct ResetEventArgs : IRootChangeEventArgs, IEquatable<ResetEventArgs>
    {
        internal ResetEventArgs(IList source)
        {
            this.Source = source;
        }

        /// <summary>Gets the collection that changed.</summary>
        public IList Source { get; }

        /// <inheritdoc />
        object IRootChangeEventArgs.Source => this.Source;

        /// <summary>Check if <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="ResetEventArgs"/>.</param>
        /// <param name="right">The right <see cref="ResetEventArgs"/>.</param>
        /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        public static bool operator ==(ResetEventArgs left, ResetEventArgs right)
        {
            return left.Equals(right);
        }

        /// <summary>Check if <paramref name="left"/> is not equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="ResetEventArgs"/>.</param>
        /// <param name="right">The right <see cref="ResetEventArgs"/>.</param>
        /// <returns>True if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        public static bool operator !=(ResetEventArgs left, ResetEventArgs right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public bool Equals(ResetEventArgs other) => Equals(this.Source, other.Source);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is ResetEventArgs other && this.Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => this.Source?.GetHashCode() ?? 0;
    }
}
