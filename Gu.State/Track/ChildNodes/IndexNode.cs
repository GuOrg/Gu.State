namespace Gu.State
{
    using System;

    internal sealed class IndexNode : IChildNode
    {
        internal readonly int Index;
        private readonly IRefCounted<ChangeTrackerNode> node;

        private IndexNode(IRefCounted<ChangeTrackerNode> node, int index)
        {
            this.Index = index;
            this.node = node;
            this.node.Value.Changed += this.OnNodeChanged;
        }

        public event EventHandler<TrackerChangedEventArgs<ChangeTrackerNode>> Changed;

        public void Dispose()
        {
            this.node.Value.Changed += this.OnNodeChanged;
            this.node.Dispose();
        }

        internal static bool TryCreate(object value, PropertiesSettings settings, int index, out IndexNode result)
        {
            IRefCounted<ChangeTrackerNode> node;
            if (ChangeTrackerNode.TryGetOrCreate(value, settings, false, out node))
            {
                result = new IndexNode(node, index);
                return true;
            }

            result = null;
            return false;
        }

        private void OnNodeChanged(object sender, TrackerChangedEventArgs<ChangeTrackerNode> e)
        {
            this.Changed?.Invoke(this, e.With(this.node.Value, this.Index));
        }
    }
}