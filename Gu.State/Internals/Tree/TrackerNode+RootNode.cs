namespace Gu.State
{
    using System;

    internal static partial class TrackerNode
    {
        private sealed class RootNode<TTracker> : INode<TTracker>, IRootNode<TTracker>
            where TTracker : ITracker
        {
            private readonly RefCountCollection<IReference, TTracker> cache;
            private readonly Lazy<DisposingMap<IReference, ChildNode<TTracker>>> children = new Lazy<DisposingMap<IReference, ChildNode<TTracker>>>(() => new DisposingMap<IReference, ChildNode<TTracker>>());

            private RootNode(object source, TTracker tracker, RefCountCollection<IReference, TTracker> cache)
            {
                this.Tracker = tracker;
                this.cache = cache;
                this.cache.TryAdd(source, tracker);
            }

            public IRootNode<TTracker> Root => this;

            public TTracker Tracker { get; }

            public INode<TTracker> AddChild(object source, Func<TTracker> trackerFactory)
            {
                var tracker = this.cache.GetOrAdd(source, trackerFactory);
                var node = new ChildNode<TTracker>(source, tracker, this);
                this.children.Value.SetValue(source, node);
                return node;
            }

            public void RemoveChild(object source)
            {
                this.children.Value.SetValue(source, null);
            }

            public void Dispose()
            {
                this.cache?.Dispose();
                if (this.children.IsValueCreated)
                {
                    this.children.Value.Dispose();
                }
            }

            internal static RootNode<TTracker> CreateRoot(object source, TTracker tracker)
            {
                return new RootNode<TTracker>(source, tracker, new RefCountCollection<IReference, TTracker>());
            }
        }
    }
}
