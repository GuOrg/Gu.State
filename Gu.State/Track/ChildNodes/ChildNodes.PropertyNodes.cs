namespace Gu.State
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    internal partial class ChildNodes<T>
    {
        private class PropertyNodes
        {
            private readonly List<IUnsubscriber<IChildNode<T>>> nodes = new();

            internal void SetValue(PropertyInfo property, IUnsubscriber<IChildNode<T>> childNode)
            {
                if (childNode is null)
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
                        else
                        {
                            this.nodes[index].Dispose();
                            this.nodes[index] = childNode;
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
                lock (this.nodes)
                {
                    for (var i = this.nodes.Count - 1; i >= 0; i--)
                    {
                        this.nodes[i].Dispose();
                        this.nodes.RemoveAt(i);
                    }
                }
            }

            internal IEnumerable<T> TrackerNodes()
            {
                lock (this.nodes)
                {
                    foreach (var unsubscriber in this.nodes)
                    {
                        yield return unsubscriber.Value.TrackerNode;
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
