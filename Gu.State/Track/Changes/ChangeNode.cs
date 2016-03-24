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

    internal sealed class ChangeNode : IRefCountable, IChangeTracker
    {
        private static readonly PropertyChangedEventArgs ChangesPropertyEventArgs = new PropertyChangedEventArgs(nameof(Changes));
        private readonly IRefCounted<ChangeTrackerNode> node;
        private readonly DisposingMap<IDisposable> children = new DisposingMap<IDisposable>();

        private int changes;

        private ChangeNode(object source, PropertiesSettings settings)
        {
            this.node = ChangeTrackerNode.GetOrCreate(this, source, settings);
            this.node.Tracker.Change += this.OnTrackerChange;
            switch (this.node.Tracker.Settings.ReferenceHandling)
            {
                case ReferenceHandling.Throw:
                    break;
                case ReferenceHandling.References:
                    break;
                case ReferenceHandling.Structural:
                case ReferenceHandling.StructuralWithReferenceLoops:
                    this.node.Tracker.PropertyChange += this.OnTrackedPropertyChange;
                    this.node.Tracker.Add += this.OnTrackedAdd;
                    this.node.Tracker.Remove += this.OnTrackedRemove;
                    this.node.Tracker.Move += this.OnTrackedMove;
                    this.node.Tracker.Reset += this.OnTrackedReset;
                    foreach (var property in this.TrackProperties)
                    {
                        this.UpdatePropertyNode(property);
                    }

                    var list = this.node.Tracker.Source as IList;
                    if (list != null)
                    {
                        var itemType = list.GetType().GetItemType();
                        if (!itemType.IsImmutable() && !this.node.Tracker.Settings.IsIgnoringDeclaringType(itemType))
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                this.UpdateIndexNode(i);
                            }
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public event EventHandler<ChangeNode> ChildChanged;

        public event EventHandler Changed;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Changes
        {
            get
            {
                return this.changes;
            }

            private set
            {
                if (value == this.changes)
                {
                    return;
                }

                this.changes = value;
                this.PropertyChanged?.Invoke(this, ChangesPropertyEventArgs);
            }
        }

        private IReadOnlyCollection<PropertyInfo> TrackProperties => this.node.Tracker.TrackProperties;

        private PropertiesSettings Settings => this.node.Tracker.Settings;

        public void Dispose()
        {
            this.node.RemoveOwner(this);
            this.node.Tracker.Change -= this.OnTrackerChange;
            this.node.Tracker.PropertyChange -= this.OnTrackedPropertyChange;
            this.node.Tracker.Add -= this.OnTrackedAdd;
            this.node.Tracker.Remove -= this.OnTrackedRemove;
            this.node.Tracker.Move -= this.OnTrackedMove;
            this.node.Tracker.Reset -= this.OnTrackedReset;
            this.children.Dispose();
        }

        internal static IRefCounted<ChangeNode> GetOrCreate(object owner, object source, PropertiesSettings settings)
        {
            Debug.Assert(source != null, "Cannot track null");
            Debug.Assert(source is INotifyPropertyChanged || source is INotifyCollectionChanged, "Must notify");
            return settings.ChangeNodes.GetOrAdd(owner, source, () => new ChangeNode(source, settings));
        }

        private void OnTrackerChange(object sender, EventArgs e)
        {
            this.Changes++;
            this.Changed?.Invoke(this, EventArgs.Empty);
            this.ChildChanged?.Invoke(this, this);
        }

        private void OnChildChange(object sender, ChangeNode originalSource)
        {
            if (ReferenceEquals(this, originalSource))
            {
                return;
            }

            this.Changes++;
            this.Changed?.Invoke(this, EventArgs.Empty);
            this.ChildChanged?.Invoke(this, originalSource);
        }

        private void OnTrackedPropertyChange(object sender, PropertyChangeEventArgs e)
        {
            if (this.TrackProperties.Contains(e.PropertyInfo) &&
               (this.Settings.ReferenceHandling == ReferenceHandling.Structural || this.Settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops))
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
            this.children.Remove(e.Index);
        }

        private void OnTrackedMove(object sender, MoveEventArgs e)
        {
            this.children.Move(e.FromIndex, e.ToIndex);
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
            var value = property.GetValue(this.node.Tracker.Source);
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
            var list = (IList)this.node.Tracker.Source;
            var value = list[index];
            if (value != null)
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
            var childNode = GetOrCreate(this, child, this.node.Tracker.Settings);
            childNode.Tracker.ChildChanged += this.OnChildChange;
            var disposable = new Disposer(() =>
                {
                    childNode.RemoveOwner(this);
                    childNode.Tracker.ChildChanged -= this.OnChildChange;
                });
            return disposable;
        }
    }
}
