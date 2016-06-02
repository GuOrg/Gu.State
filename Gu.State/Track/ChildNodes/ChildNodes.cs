namespace Gu.State
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    internal partial class ChildNodes
    {
        private static readonly ConcurrentQueue<ChildNodes> Cache = new ConcurrentQueue<ChildNodes>();
        private readonly PropertyNodes propertyNodes = new PropertyNodes();
        private readonly IndexNodes indexNodes = new IndexNodes();

        private ChildNodes()
        {
        }

        public static IBorrowed<ChildNodes> Borrow()
        {
            ChildNodes childNodes;
            if (Cache.TryDequeue(out childNodes))
            {
                return Borrowed.Create(childNodes, Return);
            }

            return Borrowed.Create(new ChildNodes(), Return);
        }

        internal static IChildNode Create(ChangeTrackerNode parent, ChangeTrackerNode node, int index)
        {
            return new IndexNode(parent, node, index);
        }

        internal static IChildNode Create(ChangeTrackerNode parent, ChangeTrackerNode node, PropertyInfo propertyInfo)
        {
            return new PropertyNode(parent, node, propertyInfo);
        }

        internal void SetValue(PropertyInfo property, IUnsubscriber<IChildNode> childNode)
        {
            this.propertyNodes.SetValue(property, childNode);
        }

        internal void Remove(PropertyInfo property)
        {
            this.propertyNodes.Remove(property);
        }

        internal void Insert(int index, IUnsubscriber<IChildNode> childNode)
        {
            this.indexNodes.Insert(index, childNode);
        }

        internal void Replace(int index, IUnsubscriber<IChildNode> childNode)
        {
            this.indexNodes.SetValue(index, childNode);
        }

        internal void Remove(int index)
        {
            this.indexNodes.Remove(index);
        }

        internal void Move(int fromIndex, int toIndex)
        {
            this.indexNodes.Move(fromIndex, toIndex);
        }

        internal void Reset(IReadOnlyList<IUnsubscriber<IChildNode>> newNodes)
        {
            this.indexNodes.Reset(newNodes);
        }

        private static void Return(ChildNodes nodes)
        {
            nodes.indexNodes.Clear();
            nodes.propertyNodes.Clear();
            Cache.Enqueue(nodes);
        }
    }
}
