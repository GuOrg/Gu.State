namespace Gu.State
{
    using System.Reflection;

    /// <summary>This is raised when a change bubbled up for a property value.</summary>
    /// <typeparam name="T">The type of tracker.</typeparam>
    public class PropertyGraphChangedEventArgs<T> : TrackerChangedEventArgs<T>
    {
        internal PropertyGraphChangedEventArgs(T node, PropertyInfo property, TrackerChangedEventArgs<T> previous = null)
            : base(node, previous)
        {
            this.Property = property;
        }

        /// <summary>Gets the property for which a change happened.</summary>
        public PropertyInfo Property { get; }
    }
}
