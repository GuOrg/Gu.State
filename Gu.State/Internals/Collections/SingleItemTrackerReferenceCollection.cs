namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class SingleItemTrackerReferenceCollection : IDisposable
    {
        private readonly List<TrackerReference> items = new List<TrackerReference>();
        private readonly object gate = new object();

        public void Dispose()
        {
            foreach (var trackerReference in this.items)
            {
                trackerReference.RefCount = 0;
                trackerReference.Tracker.Dispose();
            }

            this.items.Clear();
        }

        internal IDisposable GetOrAdd(object item, Func<IDisposable> creator)
        {
            lock (this.gate)
            {
                var match = this.items.SingleOrDefault(x => ReferenceEquals(x.Item, item));
                if (match != null)
                {
                    match.RefCount++;
                    return match;
                }

                var tracker = creator();
                var trackerReference = new TrackerReference(item, tracker, this);
                this.items.Add(trackerReference);
                return trackerReference;
            }
        }

        internal void Remove(object item)
        {
            lock (this.gate)
            {
                var match = this.items.SingleOrDefault(x => ReferenceEquals(x.Item, item));
                if (match != null)
                {
                    match.RefCount--;
                    if (match.RefCount == 0)
                    {
                        match.Dispose();
                        this.items.Remove(match);
                    }
                }
            }
        }

        private sealed class TrackerReference : IDisposable
        {
            internal readonly object Item;
            internal readonly IDisposable Tracker;
            internal int RefCount;
            private readonly SingleItemTrackerReferenceCollection parent;

            public TrackerReference(object item, IDisposable tracker, SingleItemTrackerReferenceCollection parent)
            {
                this.Item = item;
                this.Tracker = tracker;
                this.parent = parent;
                this.RefCount = 1;
            }

            public void Dispose()
            {
                lock (this.parent.gate)
                {
                    this.RefCount--;
                    if (this.RefCount == 0)
                    {
                        this.Tracker.Dispose();
                        this.parent.Remove(this);
                    }
                }
            }
        }
    }
}