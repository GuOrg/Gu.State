namespace Gu.State
{
    using System;

    internal sealed class DirtyTrackerNode
    {
        private readonly IRefCounted<ChangeTrackerNode> xNode;
        private readonly IRefCounted<ChangeTrackerNode> yNode;
        private readonly DisposingMap<IDisposable> children = new DisposingMap<IDisposable>();

        private DirtyTrackerNode(IRefCounted<ChangeTrackerNode> xNode, IRefCounted<ChangeTrackerNode> yNode)
        {
            this.xNode = xNode;
            this.yNode = yNode;
        }
    }
}
