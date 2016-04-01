namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class TwoItemsTrackerReferenceCollection<T> : IDisposable
         where T : IDisposable
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

        internal IDisposable GetOrAdd(object item1, object item2, Func<T> creator)
        {
            lock (this.gate)
            {
                var match = this.Find(item1, item2);
                if (match != null)
                {
                    match.RefCount++;
                    return match.Tracker;
                }

                var tracker = creator();
                var trackerReference = new TrackerReference(item1, item2, tracker, this);
                this.items.Add(trackerReference);
                return trackerReference;
            }
        }

        internal void Remove(object item1, object item2)
        {
            lock (this.gate)
            {
                var match = this.Find(item1, item2);
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

        private TrackerReference Find(object item1, object item2)
        {
            return this.items.SingleOrDefault(x => ReferenceEquals(x.Item1, item1) && ReferenceEquals(x.Item2, item2));
        }

        private void Remove(TrackerReference trackerReference)
        {
            this.items.Remove(trackerReference);
        }

        private sealed class TrackerReference : IDisposable
        {
            internal readonly object Item1;
            internal readonly object Item2;
            internal readonly T Tracker;
            internal int RefCount;
            private readonly TwoItemsTrackerReferenceCollection<T> parent;

            public TrackerReference(object item1, object item2, T tracker, TwoItemsTrackerReferenceCollection<T> parent)
            {
                this.Item1 = item1;
                this.Item2 = item2;
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