namespace Gu.State
{
    using System;
    using System.Collections;

    /// <summary>This is raised when an element was added to a notifying collection.</summary>
    public struct AddEventArgs : IRootChangeEventArgs, IEquatable<AddEventArgs>
    {
        internal AddEventArgs(IList source, int index)
        {
            this.Index = index;
            this.Source = source;
        }

        /// <summary>Gets the collection that changed.</summary>
        public IList Source { get; }

        /// <inheritdoc />
        object IRootChangeEventArgs.Source => this.Source;

        /// <summary>Gets the index at which an element was added. </summary>
        public int Index { get; }

        /// <summary>Check if <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="AddEventArgs"/>.</param>
        /// <param name="right">The right <see cref="AddEventArgs"/>.</param>
        /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        public static bool operator ==(AddEventArgs left, AddEventArgs right)
        {
            return left.Equals(right);
        }

        /// <summary>Check if <paramref name="left"/> is not equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="AddEventArgs"/>.</param>
        /// <param name="right">The right <see cref="AddEventArgs"/>.</param>
        /// <returns>True if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        public static bool operator !=(AddEventArgs left, AddEventArgs right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public bool Equals(AddEventArgs other) => Equals(this.Source, other.Source) &&
                                                  this.Index == other.Index;

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is AddEventArgs other &&
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
