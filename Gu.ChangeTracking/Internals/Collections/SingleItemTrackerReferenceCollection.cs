namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class SingleItemTrackerReferenceCollection
    {
        private readonly List<TrackerReference> items = new List<TrackerReference>();
        private readonly object gate = new object();

        internal IDisposable GetOrAdd(object item, Func<IDisposable> creator)
        {
            lock (this.gate)
            {
                var match = this.items.SingleOrDefault(x => ReferenceEquals(x.Item, item));
                if (match != null)
                {
                    match.RefCount++;
                    return match.Tracker;
                }

                var tracker = creator();
                this.items.Add(new TrackerReference(item, tracker));
                return tracker;
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

            public TrackerReference(object item, IDisposable tracker)
            {
                this.Item = item;
                this.Tracker = tracker;
                this.RefCount = 1;
            }

            public void Dispose()
            {
                this.Tracker.Dispose();
            }
        }
    }
}