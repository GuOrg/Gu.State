namespace Gu.State
{
    using System;

    internal static partial class TrackerNode
    {
        private sealed class ChildNode<TTracker> : INode<TTracker>
            where TTracker : ITracker
        {
            private readonly object source;
            private readonly Lazy<DisposingMap<IReference, ChildNode<TTracker>>> children = new Lazy<DisposingMap<IReference, ChildNode<TTracker>>>(() => new DisposingMap<IReference, ChildNode<TTracker>>());

            internal ChildNode(object source, TTracker tracker, INode<TTracker> parent)
            {
                this.source = source;
                this.Tracker = tracker;
                this.Parent = parent;
                this.Tracker.Changed += this.OnTrackerChanged;
            }

            public IRootNode<TTracker> Root
            {
                get
                {
                    var rootNode = this.Parent as IRootNode<TTracker>;
                    if (rootNode != null)
                    {
                        return rootNode;
                    }

                    return this.Parent.Root;
                }
            }

            public TTracker Tracker { get; }

            internal INode<TTracker> Parent { get; }

            public INode<TTracker> AddChild(object source, Func<TTracker> trackerFactory)
            {
                var tracker = this.Root.Cache.GetOrAdd(source, trackerFactory);
                var node = new ChildNode<TTracker>(source, tracker, this);
                this.children.Value.SetValue(source, node);
                return node;
            }

            public void RemoveChild(TKey key)
            {
                this.children.Value.SetValue(key, null);
            }

            public void Dispose()
            {
                this.Parent.RemoveChild(this.source);
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
