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
        private readonly IBorrowed<DisposingMap<IUnsubscriber<IRefCounted<ChangeTrackerNode>>>> children;

        private ChangeTrackerNode(object source, PropertiesSettings settings, bool isRoot)
        {
            this.refcountedNode = ChangeNode.GetOrCreate(source, settings, isRoot);
            this.refcountedNode.Value.Change += this.OnTrackerChange;
            this.children = DisposingMap<IUnsubscriber<IRefCounted<ChangeTrackerNode>>>.Borrow();
        }

        public event EventHandler<TrackerChangedEventArgs<ChangeTrackerNode>> Changed;

        private IReadOnlyCollection<PropertyInfo> TrackProperties => this.refcountedNode.Value.TrackProperties;

        private IList SourceList => (IList)this.Source;

        private object Source => this.refcountedNode.Value.Source;

        private DisposingMap<IUnsubscriber<IRefCounted<ChangeTrackerNode>>> Children => this.children.Value;

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
            var settings = this.Settings;
            switch (settings.ReferenceHandling)
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

                    var list = this.Source as IList;
                    if (list != null)
                    {
                        var itemType = list.GetType().GetItemType();
                        if (!settings.IsImmutable(itemType) && !settings.IsIgnoringDeclaringType(itemType))
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
            this.Changed?.Invoke(this, GraphChangeEventArgs.Create(this, null));
        }

        private void OnChildChanged(object sender, TrackerChangedEventArgs<ChangeTrackerNode> e)
        {
            if (e.Contains(this))
            {
                return;
            }

            this.Changed?.Invoke(this, e.With(this, null));
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
                this.Children.Remove(e.Index);
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
                this.Children.Move(e.FromIndex, e.ToIndex);
            }
        }

        private void OnTrackedReset(object sender, ResetEventArgs e)
        {
            using (var borrow = ListPool<IUnsubscriber<IRefCounted<ChangeTrackerNode>>>.Borrow())
            {
                foreach (var newItem in e.NewItems)
                {
                    borrow.Value.Add(this.CreateChild(newItem));
                }

                this.Children.Reset(borrow.Value);
            }
        }

        private void UpdatePropertyNode(PropertyInfo property)
        {
            var value = property.GetValue(this.Source);
            if (value != null)
            {
                var child = this.CreateChild(value);
                this.Children.SetValue(property, child);
            }
            else
            {
                this.Children.SetValue(property, null);
            }
        }

        private void UpdateIndexNode(int index)
        {
            var value = this.SourceList[index];
            if (value != null && !this.Settings.IsImmutable(value.GetType()))
            {
                var child = this.CreateChild(value);
                this.Children.SetValue(index, child);
            }
            else
            {
                this.Children.SetValue(index, null);
            }
        }

        private IUnsubscriber<IRefCounted<ChangeTrackerNode>> CreateChild(object child)
        {
            var childNode = GetOrCreate(child, this.refcountedNode.Value.Settings, false);
            childNode.Value.Changed += this.OnChildChanged;
            return childNode.UnsubscribeAndDispose(x => x.Value.Changed -= this.OnChildChanged);
        }
    }
}
