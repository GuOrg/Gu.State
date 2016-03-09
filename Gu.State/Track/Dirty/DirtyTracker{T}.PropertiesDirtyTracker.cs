namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public partial class DirtyTracker<T>
    {
        private sealed class PropertiesDirtyTracker : IDisposable
        {
            private static readonly IEnumerable<PropertyInfo> IndexerPropertySingletonCollection = new[] { ItemDirtyTracker.IndexerProperty };

            private readonly INotifyPropertyChanged x;
            private readonly INotifyPropertyChanged y;
            private readonly DirtyTracker<T> parent;
            private readonly PropertyCollection propertyTrackers;

            private PropertiesDirtyTracker(INotifyPropertyChanged x, INotifyPropertyChanged y, DirtyTracker<T> parent)
            {
                this.x = x;
                this.y = y;
                this.parent = parent;
                x.PropertyChanged += this.OnTrackedPropertyChanged;
                y.PropertyChanged += this.OnTrackedPropertyChanged;
                List<PropertyCollection.PropertyAndDisposable> items = null;
                foreach (var propertyInfo in x.GetType()
                                              .GetProperties(parent.Settings.BindingFlags))
                {
                    if (parent.Settings.IsIgnoringProperty(propertyInfo))
                    {
                        continue;
                    }

                    var tracker = this.CreatePropertyTracker(propertyInfo);
                    if (items == null)
                    {
                        items = new List<PropertyCollection.PropertyAndDisposable>();
                    }

                    items.Add(new PropertyCollection.PropertyAndDisposable(propertyInfo, tracker));
                    if (tracker.IsDirty)
                    {
                        parent.diff.Add(propertyInfo);
                    }
                    else
                    {
                        parent.diff.Remove(propertyInfo);
                    }
                }

                if (items != null)
                {
                    this.propertyTrackers = new PropertyCollection(items);
                }
            }

            public void Dispose()
            {
                this.x.PropertyChanged -= this.OnTrackedPropertyChanged;
                this.y.PropertyChanged -= this.OnTrackedPropertyChanged;
                this.propertyTrackers?.Dispose();
            }

            internal static PropertiesDirtyTracker Create(
                INotifyPropertyChanged x,
                INotifyPropertyChanged y,
                DirtyTracker<T> parent)
            {
                if (x == null && y == null)
                {
                    return null;
                }

                if (ReferenceEquals(x, y))
                {
                    return null;
                }

                var type = x?.GetType() ?? y.GetType();
                if (x == null || y == null)
                {
                    var propertyInfos = type.GetProperties(parent.Settings.BindingFlags)
                                            .Where(p => !parent.Settings.IsIgnoringProperty(p));
                    parent.diff.UnionWith(propertyInfos);
                    return null;
                }

                if (type.IsImmutable())
                {
                    parent.diff.IntersectWith(IndexerPropertySingletonCollection);
                    var before = parent.diff.Count;
                    foreach (var propertyInfo in type.GetProperties(parent.Settings.BindingFlags))
                    {
                        if (parent.Settings.IsIgnoringProperty(propertyInfo))
                        {
                            continue;
                        }

                        var xv = propertyInfo.GetValue(x);
                        var yv = propertyInfo.GetValue(y);

                        if (!EqualBy.PropertyValues(xv, yv, parent.Settings))
                        {
                            parent.diff.Add(propertyInfo);
                        }
                    }

                    parent.NotifyChanges(before);
                }

                return new PropertiesDirtyTracker(x, y, parent);
            }

            /// <summary>
            /// Clears the <see cref="Diff"/> and calculates a new.
            /// Notifies if there are changes.
            /// </summary>
            private void Reset()
            {
                var before = this.parent.diff.Count;
                this.parent.diff.IntersectWith(IndexerPropertySingletonCollection);
                foreach (var propertyInfo in this.x.GetType()
                                                 .GetProperties(this.parent.Settings.BindingFlags))
                {
                    if (this.parent.Settings.IsIgnoringProperty(propertyInfo))
                    {
                        continue;
                    }

                    var propertyTracker = this.CreatePropertyTracker(propertyInfo);
                    this.propertyTrackers[propertyInfo] = propertyTracker;
                    if (propertyTracker.IsDirty)
                    {
                        this.parent.diff.Add(propertyInfo);
                    }
                    else
                    {
                        this.parent.diff.Remove(propertyInfo);
                    }
                }

                this.parent.NotifyChanges(before);
            }

            private void OnTrackedPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (string.IsNullOrEmpty(e.PropertyName))
                {
                    this.Reset();
                    return;
                }

                var propertyInfo = sender.GetType()
                                         .GetProperty(e.PropertyName, this.parent.Settings.BindingFlags);

                if (this.parent.Settings.IsIgnoringProperty(propertyInfo))
                {
                    return;
                }

                var before = this.parent.diff.Count;
                var propertyTracker = this.CreatePropertyTracker(propertyInfo);
                this.propertyTrackers[propertyInfo] = propertyTracker;
                if (propertyTracker.IsDirty)
                {
                    this.parent.diff.Add(propertyInfo);
                }
                else
                {
                    this.parent.diff.Remove(propertyInfo);
                }

                this.parent.NotifyChanges(before);
            }

            private IDirtyTrackerNode CreatePropertyTracker(PropertyInfo propertyInfo)
            {
                var xv = propertyInfo.GetValue(this.x);
                var yv = propertyInfo.GetValue(this.y);
                if (xv == null && yv == null)
                {
                    return NeverDirtyNode.For(propertyInfo);
                }

                if (xv == null || yv == null)
                {
                    return AlwaysDirtyNode.For(propertyInfo);
                }

                if (xv.GetType().IsEquatable())
                {
                    return Equals(xv, yv)
                               ? (IDirtyTrackerNode)NeverDirtyNode.For(ItemDirtyTracker.IndexerProperty)
                               : AlwaysDirtyNode.For(ItemDirtyTracker.IndexerProperty);
                }

                if (xv.GetType().IsImmutable())
                {
                    return EqualBy.PropertyValues(xv, yv, this.parent.Settings)
                               ? (IDirtyTrackerNode)NeverDirtyNode.For(ItemDirtyTracker.IndexerProperty)
                               : AlwaysDirtyNode.For(ItemDirtyTracker.IndexerProperty);
                }

                switch (this.parent.Settings.ReferenceHandling)
                {
                    case ReferenceHandling.Throw:
                        var message = $"{typeof(Track).Name} does not support tracking an item of type {xv.GetType().Name}. Specify {typeof(ReferenceHandling).Name} if you want to track a graph";
                        throw new NotSupportedException(message);
                    case ReferenceHandling.References:
                        return ReferenceEquals(xv, yv)
                                   ? (IDirtyTrackerNode)NeverDirtyNode.For(ItemDirtyTracker.IndexerProperty)
                                   : AlwaysDirtyNode.For(ItemDirtyTracker.IndexerProperty);
                    case ReferenceHandling.Structural:
                        return new PropertyDirtyTracker((INotifyPropertyChanged)xv, (INotifyPropertyChanged)yv, this.parent, propertyInfo);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

    }
}
