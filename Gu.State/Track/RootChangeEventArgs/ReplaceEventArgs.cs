namespace Gu.State
{
    using System;
    using System.Collections;

    /// <summary>This is raised when an element was replaced in a notifying collection.</summary>
    public readonly struct ReplaceEventArgs : IRootChangeEventArgs, IEquatable<ReplaceEventArgs>
    {
        internal ReplaceEventArgs(IList source, int index)
        {
            this.Index = index;
            this.Source = source;
        }

        /// <summary>Gets the collection that changed.</summary>
        public IList Source { get; }

        /// <inheritdoc />
        object IRootChangeEventArgs.Source => this.Source;

        /// <summary>Gets the index at which an element was replaced. </summary>
        public int Index { get; }

        /// <summary>Check if <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="ReplaceEventArgs"/>.</param>
        /// <param name="right">The right <see cref="ReplaceEventArgs"/>.</param>
        /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        public static bool operator ==(ReplaceEventArgs left, ReplaceEventArgs right)
        {
            return left.Equals(right);
        }

        /// <summary>Check if <paramref name="left"/> is not equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="ReplaceEventArgs"/>.</param>
        /// <param name="right">The right <see cref="ReplaceEventArgs"/>.</param>
        /// <returns>True if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        public static bool operator !=(ReplaceEventArgs left, ReplaceEventArgs right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public bool Equals(ReplaceEventArgs other) => Equals(this.Source, other.Source) &&
                                                      this.Index == other.Index;

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is ReplaceEventArgs other &&
                                                   this.Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Source != null ? this.Source.GetHashCode() : 0) * 397) ^ this.Index;
            }
        }
    }
}
