namespace Gu.State
{
    using System;
    using System.Reflection;

    internal partial class ChildNodes<T>
    {
        private sealed class PropertyNode : IChildNode<T>
        {
            internal readonly PropertyInfo PropertyInfo;
            private readonly T parent;
            private readonly IRefCounted<T> node;

            internal PropertyNode(T parent, T node, PropertyInfo propertyInfo)
            {
                this.parent = parent;
                this.PropertyInfo = propertyInfo;
                this.node = node.RefCount();
                this.node.Value.Changed += this.OnNodeChanged;
            }

            public event EventHandler<TrackerChangedEventArgs<T>> Changed;

            public T TrackerNode => this.node.Value;

            public void Dispose()
            {
                this.node.Value.Changed -= this.OnNodeChanged;
                this.node.Dispose();
            }

            private void OnNodeChanged(object sender, TrackerChangedEventArgs<T> e)
            {
                this.Changed?.Invoke(this, e.With(this.parent, this.PropertyInfo));
            }
        }
    }
}
