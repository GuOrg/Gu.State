namespace Gu.State
{
    using System;

    internal static partial class TrackerNode
    {
        private sealed class RootNode<TKey, TTracker> : INode<TKey, TTracker>, IRootNode<TKey, TTracker>
            where TKey : IReference
            where TTracker : ITracker
        {
            private readonly Lazy<DisposingMap<TKey, ChildNode<TKey, TTracker>>> children = new Lazy<DisposingMap<TKey, ChildNode<TKey, TTracker>>>(() => new DisposingMap<TKey, ChildNode<TKey, TTracker>>());

            private RootNode(TKey key, TTracker tracker, RefCountCollection<TKey, TTracker> cache)
            {
                this.Tracker = tracker;
                this.Cache = cache;
                this.Cache.TryAdd(key, tracker);
            }

            public IRootNode<TKey, TTracker> Root => this;

            public TTracker Tracker { get;  }

            public RefCountCollection<TKey, TTracker> Cache { get; }

            public void AddChild(TKey key, Func<TTracker> trackerFactory)
            {
                var tracker = this.Cache.GetOrAdd(key, trackerFactory);
                var node = new ChildNode<TKey, TTracker>(key, tracker, this);
                this.children.Value.SetValue(key, node);
            }

            public void RemoveChild(TKey key)
            {
                this.children.Value.SetValue(key, null);
            }

            internal static RootNode<TKey, TTracker> CreateRoot(TKey key, TTracker tracker)
            {
                return new RootNode<TKey, TTracker>(key, tracker, new RefCountCollection<TKey, TTracker>());
            }

            public void Dispose()
            {
                this.Cache?.Dispose();
                if (this.children.IsValueCreated)
                {
                    this.children.Value.Dispose();
                }
            }
        }

    }
}
