namespace Gu.State
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    internal partial class ChildNodes<T>
        where T : class, ITrackerNode<T>
    {
        private static readonly ConcurrentQueue<ChildNodes<T>> Cache = new ConcurrentQueue<ChildNodes<T>>();
        private readonly PropertyNodes propertyNodes = new PropertyNodes();
        private readonly IndexNodes indexNodes = new IndexNodes();

        private ChildNodes()
        {
        }

        public static IBorrowed<ChildNodes<T>> Borrow()
        {
            ChildNodes<T> childNodes;
            if (Cache.TryDequeue(out childNodes))
            {
                return Borrowed.Create(childNodes, Return);
            }

            return Borrowed.Create(new ChildNodes<T>(), Return);
        }

        internal static IChildNode<T> CreateChildNode(T parent, T node, int index)
        {
            return new IndexNode(parent, node, index);
        }

        internal static IChildNode<T> CreateChildNode(T parent, T node, PropertyInfo propertyInfo)
        {
            return new PropertyNode(parent, node, propertyInfo);
        }

        internal IEnumerable<T> AllChildren()
        {
            foreach (var node in this.propertyNodes.TrackerNodes())
            {
                yield return node;
            }

            foreach (var node in this.indexNodes.TrackerNodes())
            {
                yield return node;
            }
        }

        internal void SetValue(PropertyInfo property, IUnsubscriber<IChildNode<T>> childNode)
        {
            this.propertyNodes.SetValue(property, childNode);
        }

        internal void Remove(PropertyInfo property)
        {
            this.propertyNodes.Remove(property);
        }

        internal void Insert(int index, IUnsubscriber<IChildNode<T>> childNode)
        {
            this.indexNodes.Insert(index, childNode);
        }

        internal void Replace(int index, IUnsubscriber<IChildNode<T>> childNode)
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

        internal void Reset(IReadOnlyList<IUnsubscriber<IChildNode<T>>> newNodes)
        {
            this.indexNodes.Reset(newNodes);
        }

        private static void Return(ChildNodes<T> nodes)
        {
            nodes.indexNodes.Clear();
            nodes.propertyNodes.Clear();
            Cache.Enqueue(nodes);
        }
    }
}
