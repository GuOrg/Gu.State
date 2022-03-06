namespace Gu.State
{
    using System;

    internal partial class ChildNodes<T>
    {
        private sealed class IndexNode : IChildNode<T>
        {
            internal int Index;
            private readonly T parent;
            private readonly IRefCounted<T> node;

            internal IndexNode(T parent, T node, int index)
            {
                this.parent = parent;
                this.Index = index;
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
                this.Changed?.Invoke(this, e.With(this.parent, this.Index));
            }
        }
    }
}
