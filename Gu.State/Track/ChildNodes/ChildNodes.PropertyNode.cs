namespace Gu.State
{
    using System;
    using System.Reflection;

    internal partial class ChildNodes
    {
        private sealed class PropertyNode : IChildNode
        {
            internal readonly PropertyInfo PropertyInfo;
            private readonly ChangeTrackerNode parent;
            private readonly IRefCounted<ChangeTrackerNode> node;

            internal PropertyNode(ChangeTrackerNode parent, IRefCounted<ChangeTrackerNode> node, PropertyInfo propertyInfo)
            {
                this.parent = parent;
                this.PropertyInfo = propertyInfo;
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
                this.Changed?.Invoke(this, e.With(this.parent, this.PropertyInfo));
            }
        }
    }
}