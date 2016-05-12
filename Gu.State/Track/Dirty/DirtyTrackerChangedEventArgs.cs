namespace Gu.State
{
    using System;

    internal class DirtyTrackerChangedEventArgs : EventArgs
    {
        internal readonly object Key;
        private readonly DirtyTrackerChangedEventArgs previous;
        private readonly DirtyTrackerNode node;

        public DirtyTrackerChangedEventArgs(
            DirtyTrackerNode node,
            object key,
            DirtyTrackerChangedEventArgs previous = null)
        {
            this.node = node;
            this.Key = key;
            this.previous = previous;
        }

        public bool Contains(DirtyTrackerNode other)
        {
            return ReferenceEquals(other, this.node) ||
                   this.previous?.Contains(other) == true;
        }

        public DirtyTrackerChangedEventArgs With(DirtyTrackerNode next, object key)
        {
            return new DirtyTrackerChangedEventArgs(next, key, this);
        }
    }
}