namespace Gu.State
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    internal partial class ChildNodes
    {
        private class IndexNodes
        {
            private readonly List<IUnsubscriber<IChildNode>> nodes = new List<IUnsubscriber<IChildNode>>();

            internal void Clear()
            {
                lock (this.nodes)
                {
                    for (var i = this.nodes.Count - 1; i >= 0; i--)
                    {
                        this.nodes[0].Dispose();
                        this.nodes.RemoveAt(i);
                    }
                }
            }

            internal void Insert(int index, IUnsubscriber<IChildNode> childNode)
            {
                if (childNode == null)
                {
                    this.Remove(index);
                }
                else
                {
                    Debug.Assert(childNode.Value is IndexNode, "childNode.Value is IndexNode");
                    lock (this.nodes)
                    {
                        var indexOf = this.IndexOf(index);
                        var insertIndex = indexOf < 0
                                              ? ~indexOf
                                              : indexOf;
                        for (var j = insertIndex; j < this.nodes.Count; j++)
                        {
                            ((IndexNode)this.nodes[j].Value).Index++;
                        }

                        this.nodes.Insert(insertIndex, childNode);
                    }
                }
            }

            internal void SetValue(int index, IUnsubscriber<IChildNode> childNode)
            {
                if (childNode == null)
                {
                    this.Remove(index);
                }
                else
                {
                    Debug.Assert(childNode.Value is IndexNode, "childNode.Value is IndexNode");
                    lock (this.nodes)
                    {
                        var indexOf = this.IndexOf(index);
                        if (indexOf < 0)
                        {
                            this.Insert(index, childNode);
                        }
                        else
                        {
                            this.nodes[indexOf].Dispose();
                            this.nodes[indexOf] = childNode;
                        }
                    }
                }
            }

            internal void Remove(int index)
            {
                lock (this.nodes)
                {
                    var i = this.IndexOf(index);
                    if (i < 0)
                    {
                        return;
                    }

                    this.nodes[i].Dispose();
                    this.nodes.RemoveAt(i);
                    for (var j = i; j < this.nodes.Count; j++)
                    {
                        ((IndexNode)this.nodes[j].Value).Index--;
                    }
                }
            }

            internal void Move(int fromIndex, int toIndex)
            {
                lock (this.nodes)
                {
                    var fi = this.IndexOf(fromIndex);
                    if (fi < 0)
                    {
                        return;
                    }

                    var ti = this.IndexOf(toIndex);
                    if (ti < 0)
                    {
                        ti = ~ti;
                    }

                    var node = this.nodes[fi];
                    this.nodes.RemoveAt(fi);

                    if (ti < fi)
                    {
                        for (var i = ti; i < fi; i++)
                        {
                            ((IndexNode)this.nodes[i].Value).Index++;
                        }
                    }
                    else
                    {
                        for (var i = fi; i < ti; i++)
                        {
                            ((IndexNode)this.nodes[i].Value).Index--;
                        }
                    }

                    ((IndexNode)node.Value).Index = toIndex;
                    this.nodes.Insert(ti, node);
                }
            }

            internal void Reset(IReadOnlyList<IUnsubscriber<IChildNode>> newNodes)
            {
                Debug.Assert(newNodes.All(x => x.Value is IndexNode), "Must be index nodes only");
                Debug.Assert(newNodes.Select(x => ((IndexNode)x.Value).Index).Distinct().Count() == newNodes.Count, "Must be disticnt nodes");
                Debug.Assert(newNodes.Select(x => ((IndexNode)x.Value).Index).IsIncreasing(), "Nodes must be sorted");
                lock (this.nodes)
                {
                    this.Clear();
                    foreach (var node in newNodes)
                    {
                        this.nodes.Add(node);
                    }
                }
            }

            private int IndexOf(int index)
            {
                for (var i = 0; i < this.nodes.Count; i++)
                {
                    if (((IndexNode)this.nodes[i].Value).Index < index)
                    {
                        continue;
                    }

                    if (((IndexNode)this.nodes[i].Value).Index == index)
                    {
                        return i;
                    }

                    return ~(i - 1);
                }

                return ~this.nodes.Count;
            }
        }
    }
}