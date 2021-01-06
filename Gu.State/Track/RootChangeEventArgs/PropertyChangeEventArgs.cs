namespace Gu.State
{
    using System;
    using System.Reflection;

    /// <summary>
    /// When trackers notify about changes.
    /// </summary>
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public readonly struct PropertyChangeEventArgs : IRootChangeEventArgs, IEquatable<PropertyChangeEventArgs>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangeEventArgs"/> struct.
        /// </summary>
        /// <param name="source">The source instance that changed.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
        public PropertyChangeEventArgs(object source, PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
            this.Source = source;
        }

        /// <inheritdoc />
        public object Source { get; }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/>.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>Check if <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="PropertyChangeEventArgs"/>.</param>
        /// <param name="right">The right <see cref="PropertyChangeEventArgs"/>.</param>
        /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        public static bool operator ==(PropertyChangeEventArgs left, PropertyChangeEventArgs right)
        {
            return left.Equals(right);
        }

        /// <summary>Check if <paramref name="left"/> is not equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="PropertyChangeEventArgs"/>.</param>
        /// <param name="right">The right <see cref="PropertyChangeEventArgs"/>.</param>
        /// <returns>True if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        public static bool operator !=(PropertyChangeEventArgs left, PropertyChangeEventArgs right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public bool Equals(PropertyChangeEventArgs other) => Equals(this.Source, other.Source) &&
                                                                      Equals(this.PropertyInfo, other.PropertyInfo);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is PropertyChangeEventArgs other &&
                                                   this.Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Source != null ? this.Source.GetHashCode() : 0) * 397) ^ (this.PropertyInfo != null ? this.PropertyInfo.GetHashCode() : 0);
            }
        }
    }
}
