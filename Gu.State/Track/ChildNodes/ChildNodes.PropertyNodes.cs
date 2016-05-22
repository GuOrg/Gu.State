namespace Gu.State
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    internal partial class ChildNodes
    {
        private class PropertyNodes
        {
            private readonly List<IUnsubscriber<IChildNode>> nodes = new List<IUnsubscriber<IChildNode>>();

            internal void SetValue(PropertyInfo property, IUnsubscriber<IChildNode> childNode)
            {
                if (childNode == null)
                {
                    this.Remove(property);
                }
                else
                {
                    Debug.Assert(childNode.Value is PropertyNode, "childNode.Value is PropertyNode");
                    lock (this.nodes)
                    {
                        var index = this.IndexOf(property);
                        if (index < 0)
                        {
                            this.nodes.Add(childNode);
                        }
                    }
                }
            }

            internal void Remove(PropertyInfo property)
            {
                lock (this.nodes)
                {
                    var index = this.IndexOf(property);
                    if (index >= 0)
                    {
                        this.nodes[index].Dispose();
                        this.nodes.RemoveAt(index);
                    }
                }
            }

            internal void Clear()
            {
                lock (this.nodes) // lock not needed here, just muting the warning
                {
                    for (var i = this.nodes.Count - 1; i >= 0; i--)
                    {
                        this.nodes[0].Dispose();
                        this.nodes.RemoveAt(i);
                    }
                }
            }

            private int IndexOf(PropertyInfo property)
            {
                for (var i = 0; i < this.nodes.Count; i++)
                {
                    if (((PropertyNode)this.nodes[i].Value).PropertyInfo == property)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }
    }
}