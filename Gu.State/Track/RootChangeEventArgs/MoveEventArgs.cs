namespace Gu.State
{
    using System;
    using System.Collections;

    /// <summary>This is raised when an element is moved in a notifying collection.</summary>
    public struct MoveEventArgs : IRootChangeEventArgs, IEquatable<MoveEventArgs>
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

        public static bool operator ==(MoveEventArgs left, MoveEventArgs right)
        {
            return left.Equals(right);
        }

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