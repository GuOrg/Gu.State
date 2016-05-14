namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal sealed class ChangeTrackerNode : IDisposable, IInitialize<ChangeTrackerNode>
    {
        private readonly IRefCounted<ChangeNode> refcountedNode;
        private readonly DisposingMap<IDisposable> children = new DisposingMap<IDisposable>();

        private ChangeTrackerNode(object source, PropertiesSettings settings, bool isRoot)
        {
            this.refcountedNode = ChangeNode.GetOrCreate(source, settings, isRoot);
            this.refcountedNode.Value.Change += this.OnTrackerChange;
        }

        public event EventHandler Changed;

        public event EventHandler<ChangeTrackerNode> BubbleChange;

        private IReadOnlyCollection<PropertyInfo> TrackProperties => this.refcountedNode.Value.TrackProperties;

        private PropertiesSettings Settings => this.refcountedNode.Value.Settings;

        public void Dispose()
        {
            this.refcountedNode.Value.Change -= this.OnTrackerChange;
            this.refcountedNode.Value.PropertyChange -= this.OnTrackedPropertyChange;
            this.refcountedNode.Value.Add -= this.OnTrackedAdd;
            this.refcountedNode.Value.Remove -= this.OnTrackedRemove;
            this.refcountedNode.Value.Replace -= this.OnTrackedReplace;
            this.refcountedNode.Value.Move -= this.OnTrackedMove;
            this.refcountedNode.Value.Reset -= this.OnTrackedReset;
            this.refcountedNode.Dispose();
            this.children.Dispose();
        }

        public ChangeTrackerNode Initialize()
        {
            switch (this.refcountedNode.Value.Settings.ReferenceHandling)
            {
                case ReferenceHandling.Throw:
                    break;
                case ReferenceHandling.References:
                    break;
                case ReferenceHandling.Structural:
                    this.refcountedNode.Value.PropertyChange += this.OnTrackedPropertyChange;
                    this.refcountedNode.Value.Add += this.OnTrackedAdd;
                    this.refcountedNode.Value.Remove += this.OnTrackedRemove;
                    this.refcountedNode.Value.Replace += this.OnTrackedReplace;
                    this.refcountedNode.Value.Move += this.OnTrackedMove;
                    this.refcountedNode.Value.Reset += this.OnTrackedReset;
                    foreach (var property in this.TrackProperties)
                    {
                        this.UpdatePropertyNode(property);
                    }

                    var list = this.refcountedNode.Value.Source as IList;
                    if (list != null)
                    {
                        var itemType = list.GetType().GetItemType();
                        if (!this.Settings.IsImmutable(itemType) && !this.refcountedNode.Value.Settings.IsIgnoringDeclaringType(itemType))
                        {
                            for (var i = 0; i < list.Count; i++)
                            {
                                this.UpdateIndexNode(i);
                            }
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return this;
        }

        internal static IRefCounted<ChangeTrackerNode> GetOrCreate(object source, PropertiesSettings settings, bool isRoot)
        {
            Debug.Assert(source != null, "Cannot track null");
            Debug.Assert(source is INotifyPropertyChanged || source is INotifyCollectionChanged, "Must notify");
            return TrackerCache.GetOrAdd(source, settings, s => new ChangeTrackerNode(s, settings, isRoot));
        }

        private void OnTrackerChange(object sender, EventArgs e)
        {
            this.Changed?.Invoke(this, EventArgs.Empty);
            this.BubbleChange?.Invoke(this, this);
        }

        private void OnBubbleChange(object sender, ChangeTrackerNode originalSource)
        {
            if (ReferenceEquals(this, originalSource))
            {
                return;
            }

            this.Changed?.Invoke(this, EventArgs.Empty);
            this.BubbleChange?.Invoke(this, originalSource);
        }

        private void OnTrackedPropertyChange(object sender, PropertyChangeEventArgs e)
        {
            if (this.TrackProperties.Contains(e.PropertyInfo) &&
               (this.Settings.ReferenceHandling == ReferenceHandling.Structural))
            {
                this.UpdatePropertyNode(e.PropertyInfo);
            }
        }

        private void OnTrackedAdd(object sender, AddEventArgs e)
        {
            this.UpdateIndexNode(e.Index);
        }

        private void OnTrackedRemove(object sender, RemoveEventArgs e)
        {
            if (!this.Settings.IsImmutable(this.refcountedNode.Value.Source.GetType().GetItemType()))
            {
                this.children.Remove(e.Index);
            }
        }

        private void OnTrackedReplace(object sender, ReplaceEventArgs e)
        {
            this.UpdateIndexNode(e.Index);
        }

        private void OnTrackedMove(object sender, MoveEventArgs e)
        {
            if (!this.Settings.IsImmutable(this.refcountedNode.Value.Source.GetType().GetItemType()))
            {
                this.children.Move(e.FromIndex, e.ToIndex);
            }
        }

        private void OnTrackedReset(object sender, ResetEventArgs e)
        {
            var newItems = new List<IDisposable>(e.NewItems.Count);
            foreach (var newItem in e.NewItems)
            {
                newItems.Add(this.CreateChild(newItem));
            }

            this.children.Reset(newItems);
        }

        private void UpdatePropertyNode(PropertyInfo property)
        {
            var value = property.GetValue(this.refcountedNode.Value.Source);
            if (value != null)
            {
                var child = this.CreateChild(value);
                this.children.SetValue(property, child);
            }
            else
            {
                this.children.SetValue(property, null);
            }
        }

        private void UpdateIndexNode(int index)
        {
            var list = (IList)this.refcountedNode.Value.Source;
            var value = list[index];
            if (value != null && !this.Settings.IsImmutable(value.GetType()))
            {
                var child = this.CreateChild(value);
                this.children.SetValue(index, child);
            }
            else
            {
                this.children.SetValue(index, null);
            }
        }

        private IDisposable CreateChild(object child)
        {
            var childNode = GetOrCreate(child, this.refcountedNode.Value.Settings, false);
            childNode.Value.BubbleChange += this.OnBubbleChange;
            var disposable = new Disposer(() =>
                {
                    childNode.Value.BubbleChange -= this.OnBubbleChange;
                    childNode.Dispose();
                });
            return disposable;
        }
    }
}
