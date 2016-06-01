namespace Gu.State
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// This is raised as a change bubbles up an object graph.
    /// Implemented as a linked list starting with the source of the change.
    /// </summary>
    /// <typeparam name="T">The type of tracker.</typeparam>
    public abstract class TrackerChangedEventArgs<T> : EventArgs
    {
        protected TrackerChangedEventArgs(
            T node,
            TrackerChangedEventArgs<T> previous = null)
        {
            this.Node = node;
            this.Previous = previous;
        }

        /// <summary>Gets the previous node that triggered the change.</summary>
        public TrackerChangedEventArgs<T> Previous { get; }

        /// <summary>Gets the current node.</summary>
        public T Node { get; }

        /// <summary>Gets the root source of the change.</summary>
        public RootChangeEventArgs<T> Root => (RootChangeEventArgs<T>)(this.Previous != null
                                                                        ? this.Previous.Root
                                                                        : this);

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