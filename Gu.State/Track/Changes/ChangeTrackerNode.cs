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
        private readonly IRefCounted<RootChanges> refcountedRootChanges;
        private readonly IBorrowed<DisposingMap<IUnsubscriber<IRefCounted<ChangeTrackerNode>>>> children;

        private ChangeTrackerNode(INotifyPropertyChanged source, PropertiesSettings settings, bool isRoot)
        {
            this.refcountedRootChanges = RootChanges.GetOrCreate(source, settings, isRoot);
            var sourceChanges = this.refcountedRootChanges.Value;
            sourceChanges.PropertyChange += this.OnSourcePropertyChange;
            if (Is.NotifyingCollection(source))
            {
                sourceChanges.Add += this.OnSourceAdd;
                sourceChanges.Remove += this.OnSourceRemove;
                sourceChanges.Replace += this.OnSourceReplace;
                sourceChanges.Move += this.OnSourceMove;
                sourceChanges.Reset += this.OnSourceReset;
            }

            this.children = DisposingMap<IUnsubscriber<IRefCounted<ChangeTrackerNode>>>.Borrow();
        }

        public event EventHandler<TrackerChangedEventArgs<ChangeTrackerNode>> Changed;

        private IReadOnlyCollection<PropertyInfo> TrackProperties => this.refcountedRootChanges.Value.TrackProperties;

        private IList SourceList => (IList)this.Source;

        private object Source => this.refcountedRootChanges.Value.Source;

        private DisposingMap<IUnsubscriber<IRefCounted<ChangeTrackerNode>>> Children => this.children.Value;

        private PropertiesSettings Settings => this.refcountedRootChanges.Value.Settings;

        public void Dispose()
        {
            var rootChanges = this.refcountedRootChanges.Value;
            rootChanges.PropertyChange -= this.OnSourcePropertyChange;
            rootChanges.Add -= this.OnSourceAdd;
            rootChanges.Remove -= this.OnSourceRemove;
            rootChanges.Replace -= this.OnSourceReplace;
            rootChanges.Move -= this.OnSourceMove;
            rootChanges.Reset -= this.OnSourceReset;
            this.refcountedRootChanges.Dispose();
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

        internal static IRefCounted<ChangeTrackerNode> GetOrCreate(INotifyPropertyChanged source, PropertiesSettings settings, bool isRoot)
        {
            Debug.Assert(source != null, "Cannot track null");
            return TrackerCache.GetOrAdd(source, settings, s => new ChangeTrackerNode(s, settings, isRoot));
        }

        private void OnChildChanged(object sender, TrackerChangedEventArgs<ChangeTrackerNode> e)
        {
            if (e.Contains(this))
            {
                return;
            }

            this.Changed?.Invoke(this, e.With(this, null));
        }

        private void OnSourcePropertyChange(object sender, PropertyChangeEventArgs e)
        {
            if (this.TrackProperties.Contains(e.PropertyInfo) &&
               (this.Settings.ReferenceHandling == ReferenceHandling.Structural))
            {
                this.UpdatePropertyNode(e.PropertyInfo);
            }
        }

        private void OnSourceAdd(object sender, AddEventArgs e)
        {
            this.UpdateIndexNode(e.Index);
        }

        private void OnSourceRemove(object sender, RemoveEventArgs e)
        {
            if (!this.Settings.IsImmutable(this.refcountedRootChanges.Value.Source.GetType().GetItemType()))
            {
                this.Children.Remove(e.Index);
            }
        }

        private void OnSourceReplace(object sender, ReplaceEventArgs e)
        {
            this.UpdateIndexNode(e.Index);
        }

        private void OnSourceMove(object sender, MoveEventArgs e)
        {
            if (!this.Settings.IsImmutable(this.refcountedRootChanges.Value.Source.GetType().GetItemType()))
            {
                this.Children.Move(e.FromIndex, e.ToIndex);
            }
        }

        private void OnSourceReset(object sender, ResetEventArgs e)
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
            var childNode = GetOrCreate(child, this.refcountedRootChanges.Value.Settings, false);
            childNode.Value.Changed += this.OnChildChanged;
            return childNode.UnsubscribeAndDispose(x => x.Value.Changed -= this.OnChildChanged);
        }
    }
}
