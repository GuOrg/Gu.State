﻿namespace Gu.State
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

        internal static bool TryCreate(ChangeTrackerNode parent, int index, out IChildNode result)
        {
            IRefCounted<ChangeTrackerNode> node;
            var value = parent.SourceList.ElementAtOrMissing(index);
            if (value == PaddedPairs.MissingItem)
            {
                result = null;
                return false;
            }

            if (ChangeTrackerNode.TryGetOrCreate(value, parent.Settings, false, out node))
            {
                result = new IndexNode(parent, node, index);
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
