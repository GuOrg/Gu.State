namespace Gu.State
{
    using System;
    using System.Collections;

    /// <summary>This is raised when an element is moved in a notifying collection.</summary>
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public readonly struct MoveEventArgs : IRootChangeEventArgs, IEquatable<MoveEventArgs>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        internal MoveEventArgs(IList source, int fromIndex, int toIndex)
        {
            this.FromIndex = fromIndex;
            this.ToIndex = toIndex;
            this.Source = source;
        }

        /// <summary>Gets the collection that changed.</summary>
        public IList Source { get; }

        /// <inheritdoc />
        object IRootChangeEventArgs.Source => this.Source;

        /// <summary>Gets the index at which an element was moved from. </summary>
        public int FromIndex { get; }

        /// <summary>Gets the index at which an element was moved to. </summary>
        public int ToIndex { get; }

        /// <summary>Check if <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="MoveEventArgs"/>.</param>
        /// <param name="right">The right <see cref="MoveEventArgs"/>.</param>
        /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        public static bool operator ==(MoveEventArgs left, MoveEventArgs right)
        {
            return left.Equals(right);
        }

        /// <summary>Check if <paramref name="left"/> is not equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="MoveEventArgs"/>.</param>
        /// <param name="right">The right <see cref="MoveEventArgs"/>.</param>
        /// <returns>True if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        public static bool operator !=(MoveEventArgs left, MoveEventArgs right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public bool Equals(MoveEventArgs other) => Equals(this.Source, other.Source) &&
                                                   this.FromIndex == other.FromIndex &&
                                                   this.ToIndex == other.ToIndex;

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is MoveEventArgs other &&
                                                   this.Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Source != null ? this.Source.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ this.FromIndex;
                hashCode = (hashCode * 397) ^ this.ToIndex;
                return hashCode;
            }
        }
    }
}
