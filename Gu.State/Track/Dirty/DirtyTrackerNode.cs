namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    internal sealed class DirtyTrackerNode : IRefCountable, INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs IsDirtyPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IsDirty));
        private readonly IRefCounted<ChangeTrackerNode> xNode;
        private readonly IRefCounted<ChangeTrackerNode> yNode;
        private readonly DisposingMap<IDisposable> children = new DisposingMap<IDisposable>();
        private readonly object gate = new object();
        private Diff diff = Diff.Empty;

        private DirtyTrackerNode(object x, object y, PropertiesSettings settings)
        {
            this.xNode = ChangeTrackerNode.GetOrCreate(this, x, settings);
            this.yNode = ChangeTrackerNode.GetOrCreate(this, y, settings);
            this.xNode.Tracker.PropertyChange += this.OnTrackedPropertyChange;
            this.yNode.Tracker.PropertyChange += this.OnTrackedPropertyChange;
            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.Throw:
                    break;
                case ReferenceHandling.References:
                    break;
                case ReferenceHandling.Structural:
                case ReferenceHandling.StructuralWithReferenceLoops:
                    foreach (var property in x.GetType().GetProperties(settings.BindingFlags))
                    {
                        this.UpdatePropertyNode(property);
                    }

                    throw new NotImplementedException("message");

                    //this.xNode.Tracker.Add += this.OnTrackedAdd;
                    //this.xNode.Tracker.Remove += this.OnTrackedRemove;
                    //this.xNode.Tracker.Move += this.OnTrackedMove;
                    //this.xNode.Tracker.Reset += this.OnTrackedReset;

                    //this.yNode.Tracker.Add += this.OnTrackedAdd;
                    //this.yNode.Tracker.Remove += this.OnTrackedRemove;
                    //this.yNode.Tracker.Move += this.OnTrackedMove;
                    //this.yNode.Tracker.Reset += this.OnTrackedReset;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<DirtyTrackerNode> ChildChanged;

        public bool IsDirty => this.Diff.IsEmpty;

        public Diff Diff
        {
            get
            {
                return this.diff;
            }

            private set
            {
                lock (this.gate)
                {
                    if (Equals(value, this.diff))
                    {
                        return;
                    }

                    var wasEmpty = this.diff.IsEmpty;
                    this.diff = value;
                    if (wasEmpty != this.diff.IsEmpty)
                    {
                        this.OnPropertyChanged(IsDirtyPropertyChangedEventArgs);
                    }
                }
            }
        }

        private IReadOnlyCollection<PropertyInfo> TrackProperties => this.xNode.Tracker.TrackProperties;

        private PropertiesSettings Settings => this.xNode.Tracker.Settings;

        public void Dispose()
        {
            this.xNode.RemoveOwner(this);
            this.xNode.Tracker.PropertyChange -= this.OnTrackedPropertyChange;
            //this.xNode.Tracker.Add -= this.OnTrackedAdd;
            //this.xNode.Tracker.Remove -= this.OnTrackedRemove;
            //this.xNode.Tracker.Move -= this.OnTrackedMove;
            //this.xNode.Tracker.Reset -= this.OnTrackedReset;

            this.yNode.RemoveOwner(this);
            this.yNode.Tracker.PropertyChange -= this.OnTrackedPropertyChange;
            //this.yNode.Tracker.Add -= this.OnTrackedAdd;
            //this.yNode.Tracker.Remove -= this.OnTrackedRemove;
            //this.yNode.Tracker.Move -= this.OnTrackedMove;
            //this.yNode.Tracker.Reset -= this.OnTrackedReset;

            this.children.Dispose();
        }

        internal static IRefCounted<DirtyTrackerNode> GetOrCreate(object owner, object x, object y, PropertiesSettings settings)
        {
            Debug.Assert(x != null, "Cannot track null");
            Debug.Assert(x is INotifyPropertyChanged || x is INotifyCollectionChanged, "Must notify");
            Debug.Assert(y != null, "Cannot track null");
            Debug.Assert(y is INotifyPropertyChanged || y is INotifyCollectionChanged, "Must notify");
            return settings.DirtyNodes.GetOrAdd(owner, new ReferencePair(x, y), () => new DirtyTrackerNode(x, y, settings));
        }

        private void OnTrackedPropertyChange(object sender, PropertyChangeEventArgs e)
        {
            this.UpdatePropertyNode(e.PropertyInfo);
        }

        private void UpdatePropertyNode(PropertyInfo propertyInfo)
        {
            if (this.Settings.IsIgnoringProperty(propertyInfo))
            {
                return;
            }

            var xValue = propertyInfo.GetValue(this.xNode.Tracker.Source);
            var yValue = propertyInfo.GetValue(this.yNode.Tracker.Source);
            if (this.TrackProperties.Contains(propertyInfo) &&
               (this.Settings.ReferenceHandling == ReferenceHandling.Structural || this.Settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops))
            {
                var refCounted = this.CreateChild(xValue, yValue, propertyInfo);
                this.children.SetValue(propertyInfo, refCounted);
            }
            else
            {
                lock (this.gate)
                {
                    this.Diff = EqualBy.PropertyValues(xValue, yValue, this.Settings)
                                    ? this.diff.Without(propertyInfo)
                                    : this.diff.With(propertyInfo, xValue, yValue);
                }
            }
        }

        private IDisposable CreateChild(object xValue, object yValue, object key)
        {
            var childNode = GetOrCreate(this, xValue, yValue, this.Settings);
            EventHandler<DirtyTrackerNode> trackerOnChildChanged = (sender, args) => this.OnChildChange(sender, args, key);
            childNode.Tracker.ChildChanged += trackerOnChildChanged;
            var disposable = new Disposer(() =>
            {
                childNode.RemoveOwner(this);
                childNode.Tracker.ChildChanged -= trackerOnChildChanged;
            });
            return disposable;
        }

        private void OnChildChange(object sender, DirtyTrackerNode originalSource, object key)
        {
            if (ReferenceEquals(this, originalSource))
            {
                return;
            }

            lock (this.gate)
            {
                var propertyInfo = key as PropertyInfo;
                if (propertyInfo != null)
                {
                    var xValue = propertyInfo.GetValue(this.xNode.Tracker.Source);
                    var yValue = propertyInfo.GetValue(this.yNode.Tracker.Source);
                    this.Diff = this.diff.With(propertyInfo, xValue, yValue);
                }

                this.ChildChanged?.Invoke(this, originalSource);
            }

            throw new NotImplementedException("message");
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }
    }
}
