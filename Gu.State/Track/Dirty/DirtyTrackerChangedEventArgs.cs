namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class DirtyTrackerChangedEventArgs : EventArgs
    {
        public DirtyTrackerChangedEventArgs(
            DirtyTrackerNode node,
            object memberOrIndex,
            DirtyTrackerChangedEventArgs previous = null)
        {
            this.Node = node;
            this.MemberOrIndex = memberOrIndex;
            this.Previous = previous;
        }

        public object MemberOrIndex { get; }

        public DirtyTrackerChangedEventArgs Previous { get; }

        public DirtyTrackerNode Node { get; }

        public DirtyTrackerChangedEventArgs Root => this.Previous != null
                                                        ? this.Previous.Root
                                                        : this;

        public bool Contains(DirtyTrackerNode other)
        {
            return ReferenceEquals(other, this.Node) ||
                   this.Previous?.Contains(other) == true;
        }

        internal DirtyTrackerChangedEventArgs With(DirtyTrackerNode next, object memberOrIndex)
        {
            return new DirtyTrackerChangedEventArgs(next, memberOrIndex, this);
        }
    }
}