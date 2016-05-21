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
        private readonly IBorrowed<ChildNodes> children;

        private ChangeTrackerNode(object source, PropertiesSettings settings, bool isRoot)
        {
            this.refcountedRootChanges = RootChanges.GetOrCreate(source, settings, isRoot);
            var sourceChanges = this.refcountedRootChanges.Value;
            this.children = ChildNodes.Borrow();
            sourceChanges.PropertyChange += this.OnSourcePropertyChange;
            if (Is.NotifyingCollection(source))
            {
                this.ItemType = this.SourceList.GetType()
                                    .GetItemType();
                sourceChanges.Add += this.OnSourceAdd;
                sourceChanges.Remove += this.OnSourceRemove;
                sourceChanges.Replace += this.OnSourceReplace;
                sourceChanges.Move += this.OnSourceMove;
                sourceChanges.Reset += this.OnSourceReset;
            }
        }

        public event EventHandler<TrackerChangedEventArgs<ChangeTrackerNode>> Changed;

        private IReadOnlyCollection<PropertyInfo> TrackProperties => this.refcountedRootChanges.Value.TrackProperties;

        private IList SourceList => (IList)this.Source;

        private Type ItemType { get; }

        private object Source => this.refcountedRootChanges.Value.Source;

        private ChildNodes Children => this.children.Value;

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

        internal static bool TryGetOrCreate(object source, PropertiesSettings settings, bool isRoot, out IRefCounted<ChangeTrackerNode> result)
        {
            var inpc = source as INotifyPropertyChanged;
            if (inpc != null)
            {
                result = GetOrCreate(inpc, settings, isRoot);
                return true;
            }

            var incc = source as INotifyCollectionChanged;
            if (incc != null)
            {
                result = GetOrCreate(incc, settings, isRoot);
                return true;
            }

            result = null;
            return false;
        }

        internal static IRefCounted<ChangeTrackerNode> GetOrCreate(INotifyPropertyChanged source, PropertiesSettings settings, bool isRoot)
        {
            Debug.Assert(source != null, "Cannot track null");
            return TrackerCache.GetOrAdd((object)source, settings, s => new ChangeTrackerNode(s, settings, isRoot));
        }

        internal static IRefCounted<ChangeTrackerNode> GetOrCreate(INotifyCollectionChanged source, PropertiesSettings settings, bool isRoot)
        {
            Debug.Assert(source != null, "Cannot track null");
            return TrackerCache.GetOrAdd((object)source, settings, s => new ChangeTrackerNode(s, settings, isRoot));
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
            this.UpdatePropertyNode(e.PropertyInfo);
            this.Changed?.Invoke(this, RootChangeEventArgs.Create(this, e));
        }

        private void OnSourceAdd(object sender, AddEventArgs e)
        {
            this.UpdateIndexNode(e.Index);
            this.Changed?.Invoke(this, RootChangeEventArgs.Create(this, e));
        }

        private void OnSourceRemove(object sender, RemoveEventArgs e)
        {
            this.Children.Remove(e.Index);
            this.Changed?.Invoke(this, RootChangeEventArgs.Create(this, e));
        }

        private void OnSourceReplace(object sender, ReplaceEventArgs e)
        {
            this.UpdateIndexNode(e.Index);
            this.Changed?.Invoke(this, RootChangeEventArgs.Create(this, e));
        }

        private void OnSourceMove(object sender, MoveEventArgs e)
        {
            this.Children.Move(e.FromIndex, e.ToIndex);
            this.Changed?.Invoke(this, RootChangeEventArgs.Create(this, e));
        }

        private void OnSourceReset(object sender, ResetEventArgs e)
        {
            if (!Is.Trackable(this.ItemType))
            {
                this.Changed?.Invoke(this, RootChangeEventArgs.Create(this, e));
                return;
            }

            using (var borrow = ListPool<IUnsubscriber<IChildNode>>.Borrow())
            {
                for (var i = 0; i < e.NewItems.Count; i++)
                {
                    var newItem = e.NewItems[i];
                    IUnsubscriber<IChildNode> childNode;
                    if (this.TryCreateChildNode(newItem, i, out childNode))
                    {
                        borrow.Value.Add(childNode);
                    }
                    else
                    {
                        borrow.Value.Add(null);
                    }
                }

                this.Children.Reset(borrow.Value);
                this.Changed?.Invoke(this, RootChangeEventArgs.Create(this, e));
            }
        }

        private void UpdatePropertyNode(PropertyInfo property)
        {
            if (this.Settings.IsIgnoringProperty(property))
            {
                return;
            }

            var value = this.Settings.GetOrCreateGetterAndSetter(property).GetValue(this.Source);
            IUnsubscriber<IChildNode> childNode;
            if (this.TryCreateChildNode(value, property, out childNode))
            {
                this.Children.SetValue(property, childNode);
            }
            else
            {
                this.Children.Remove(property);
            }
        }

        private void UpdateIndexNode(int index)
        {
            var value = this.SourceList[index];
            IUnsubscriber<IChildNode> childNode;
            if (this.TryCreateChildNode(value, index, out childNode))
            {
                this.Children.SetValue(index, childNode);
            }
            else
            {
                this.Children.Remove(index);
            }
        }

        private bool TryCreateChildNode(object value, PropertyInfo propertyInfo, out IUnsubscriber<IChildNode> result)
        {
            IChildNode propertyNode;
            if (ChildNodes.TryCreate(value, this.Settings, propertyInfo, out propertyNode))
            {
                propertyNode.Changed += this.OnChildChanged;
                result = propertyNode.UnsubscribeAndDispose(n => n.Changed -= this.OnChildChanged);
                return true;
            }

            result = null;
            return false;
        }

        private bool TryCreateChildNode(object value, int index, out IUnsubscriber<IChildNode> result)
        {
            IChildNode indexNode;
            if (ChildNodes.TryCreate(value, this.Settings, index, out indexNode))
            {
                indexNode.Changed += this.OnChildChanged;
                result = indexNode.UnsubscribeAndDispose(n => n.Changed -= this.OnChildChanged);
                return true;
            }

            result = null;
            return false;
        }
    }
}
