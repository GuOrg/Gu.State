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

        internal static bool TryCreate(object value, PropertiesSettings settings, int index, out IChildNode result)
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

        internal static bool TryCreate(ChangeTrackerNode parent, PropertyInfo propertyInfo, out IChildNode result)
        {
            IRefCounted<ChangeTrackerNode> node;
            var getter = parent.Settings.GetOrCreateGetterAndSetter(propertyInfo);
            var value = getter.GetValue(parent.Source);
            if (ChangeTrackerNode.TryGetOrCreate(value, parent.Settings, false, out node))
            {
                result = new PropertyNode(parent, node, propertyInfo);
                return true;
            }

            result = null;
            return false;
        }

        internal void SetValue(PropertyInfo property, IUnsubscriber<IChildNode> childNode)
        {
            this.propertyNodes.SetValue(property, childNode);
        }

        internal void Remove(PropertyInfo property)
        {
            this.propertyNodes.Remove(property);
        }

        internal void SetValue(int index, IUnsubscriber<IChildNode> childNode)
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
