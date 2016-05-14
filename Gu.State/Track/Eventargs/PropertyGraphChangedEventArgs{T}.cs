namespace Gu.State
{
    using System.Reflection;

    internal class PropertyGraphChangedEventArgs<T> : TrackerChangedEventArgs<T>
    {
        public PropertyGraphChangedEventArgs(T node, PropertyInfo property, TrackerChangedEventArgs<T> previous = null)
            : base(node, previous)
        {
            this.Property = property;
        }

        public PropertyInfo Property { get; }
    }
}