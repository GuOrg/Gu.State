namespace Gu.State
{
    using System;

    internal static partial class TrackerNode
    {
        private sealed class ChildNode<TKey, TTracker> : INode<TKey, TTracker>
            where TKey : IReference
            where TTracker : ITracker
        {
            private readonly TKey key;
            private readonly Lazy<DisposingMap<TKey, ChildNode<TKey, TTracker>>> children = new Lazy<DisposingMap<TKey, ChildNode<TKey, TTracker>>>(() => new DisposingMap<TKey, ChildNode<TKey, TTracker>>());

            internal ChildNode(TKey key, TTracker tracker, INode<TKey, TTracker> parent)
            {
                this.key = key;
                this.Tracker = tracker;
                this.Parent = parent;
                this.Tracker.Changed += this.OnTrackerChanged;
            }

            public IRootNode<TKey, TTracker> Root
            {
                get
                {
                    var rootNode = this.Parent as IRootNode<TKey, TTracker>;
                    if (rootNode != null)
                    {
                        return rootNode;
                    }

                    return this.Parent.Root;
                }
            }

            public TTracker Tracker { get; }

            internal INode<TKey, TTracker> Parent { get; }

            public void AddChild(TKey key, Func<TTracker> trackerFactory)
            {
                var tracker = this.Root.Cache.GetOrAdd(key, trackerFactory);
                var node = new ChildNode<TKey, TTracker>(key, tracker, this);
                this.children.Value.SetValue(key, node);
            }

            public void RemoveChild(TKey key)
            {
                this.children.Value.SetValue(key, null);
            }

            public void Dispose()
            {
                this.Parent.RemoveChild(this.key);
                this.Tracker.Changed += this.OnTrackerChanged;
                if (this.children.IsValueCreated)
                {
                    this.children.Value.Dispose();
                }
            }

            private void OnTrackerChanged(object sender, EventArgs e)
            {
                this.Parent.Tracker.ChildChanged(this.Tracker);
            }
        }
    }
}
