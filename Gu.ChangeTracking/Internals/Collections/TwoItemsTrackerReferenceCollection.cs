namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class TwoItemsTrackerReferenceCollection
    {
        private readonly List<TrackerReference> items = new List<TrackerReference>();
        private readonly object gate = new object();

        internal IDisposable GetOrAdd(object item1, object item2, Func<IDisposable> creator)
        {
            lock (this.gate)
            {
                var match = this.items.SingleOrDefault(x => ReferenceEquals(x.Item1, item1));
                if (match != null)
                {
                    match.RefCount++;
                    return match.Tracker;
                }

                var tracker = creator();
                this.items.Add(new TrackerReference(item1, item2, tracker));
                return tracker;
            }
        }

        internal void Remove(object item1, object item2)
        {
            lock (this.gate)
            {
                var match = this.items.SingleOrDefault(x => ReferenceEquals(x.Item1, item1) && ReferenceEquals(x.Item2, item2));
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
            internal readonly object Item1;
            internal readonly object Item2;
            internal readonly IDisposable Tracker;
            internal int RefCount;

            public TrackerReference(object item1, object item2, IDisposable tracker)
            {
                this.Item1 = item1;
                this.Item2 = item2;
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