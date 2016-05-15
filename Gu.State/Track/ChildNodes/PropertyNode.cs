namespace Gu.State
{
    using System;
    using System.Reflection;

    internal sealed class PropertyNode : IChildNode
    {
        internal readonly PropertyInfo PropertyInfo;
        private readonly IRefCounted<ChangeTrackerNode> node;

        private PropertyNode(IRefCounted<ChangeTrackerNode> node, PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
            this.node = node;
            this.node.Value.Changed += this.OnNodeChanged;
        }

        public event EventHandler<TrackerChangedEventArgs<ChangeTrackerNode>> Changed;

        public void Dispose()
        {
            this.node.Value.Changed += this.OnNodeChanged;
            this.node.Dispose();
        }

        internal static bool TryCreate(object value, PropertiesSettings settings, PropertyInfo propertyInfo, out  PropertyNode result)
        {
            IRefCounted<ChangeTrackerNode> node;
            if (ChangeTrackerNode.TryGetOrCreate(value, settings, false, out node))
            {
                result = new PropertyNode(node, propertyInfo);
                return true;
            }

            result = null;
            return false;
        }

        private void OnNodeChanged(object sender, TrackerChangedEventArgs<ChangeTrackerNode> e)
        {
            this.Changed?.Invoke(this, e.With(this.node.Value, this.PropertyInfo));
        }
    }
}