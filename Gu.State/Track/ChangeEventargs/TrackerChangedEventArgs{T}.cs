namespace Gu.State
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    public abstract class TrackerChangedEventArgs<T> : EventArgs
    {
        protected TrackerChangedEventArgs(
            T node,
            TrackerChangedEventArgs<T> previous = null)
        {
            this.Node = node;
            this.Previous = previous;
        }

        public TrackerChangedEventArgs<T> Previous { get; }

        public T Node { get; }

        public TrackerChangedEventArgs<T> Root => this.Previous != null
                                                      ? this.Previous.Root
                                                      : this;

        internal bool Contains(T node)
        {
            return ReferenceEquals(node, this.Node) ||
                   this.Previous?.Contains(node) == true;
        }

        internal TrackerChangedEventArgs<T> With(T next, int index)
        {
            return new ItemGraphChangedEventArgs<T>(next, index, this);
        }

        internal TrackerChangedEventArgs<T> With(T next, PropertyInfo property)
        {
            Debug.Assert(property != null, "property == null");
            return new PropertyGraphChangedEventArgs<T>(next, property, this);
        }
    }
}