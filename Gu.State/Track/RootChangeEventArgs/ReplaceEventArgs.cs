namespace Gu.State
{
    using System;
    using System.Collections;

    /// <summary>This is raised when an element was replaced in a notifying collection.</summary>
    public struct ReplaceEventArgs : IRootChangeEventArgs, IEquatable<ReplaceEventArgs>
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

        public static bool operator ==(ReplaceEventArgs left, ReplaceEventArgs right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReplaceEventArgs left, ReplaceEventArgs right)
        {
            return !left.Equals(right);
        }

        /// <summary>Gets the index at which an element was replaced. </summary>
        public int Index { get; }

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