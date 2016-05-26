namespace Gu.State
{
    using System;

    internal partial class ChildNodes
    {
        private sealed class IndexNode : IChildNode
        {
            internal int Index;
            private readonly ChangeTrackerNode parent;
            private readonly IRefCounted<ChangeTrackerNode> node;

            internal IndexNode(ChangeTrackerNode parent, IRefCounted<ChangeTrackerNode> node, int index)
            {
                this.parent = parent;
                this.Index = index;
                this.node = node;
                this.node.Value.Changed += this.OnNodeChanged;
            }

            public event EventHandler<TrackerChangedEventArgs<ChangeTrackerNode>> Changed;

            public void Dispose()
            {
                this.node.Value.Changed -= this.OnNodeChanged;
                this.node.Dispose();
            }

            private void OnNodeChanged(object sender, TrackerChangedEventArgs<ChangeTrackerNode> e)
            {
                this.Changed?.Invoke(this, e.With(this.parent, this.Index));
            }
        }
    }
}