namespace Gu.State
{
    using System;

    internal class TrackerChangedEventArgs<T> : EventArgs
    {
        public TrackerChangedEventArgs(
            T node,
            object memberOrIndex,
            TrackerChangedEventArgs<T> previous = null)
        {
            this.Node = node;
            this.MemberOrIndex = memberOrIndex;
            this.Previous = previous;
        }

        public object MemberOrIndex { get; }

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

        internal TrackerChangedEventArgs<T> With(T next, object memberOrIndex)
        {
            return new TrackerChangedEventArgs<T>(next, memberOrIndex, this);
        }
    }
}