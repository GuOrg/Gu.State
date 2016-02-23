namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    /// <summary>
    /// Tracks changes in a graph.
    /// Listens to nested Property and collection changes.
    /// </summary>
    public class ChangeTracker : IChangeTracker
    {
        private static readonly PropertyInfo ChangesPropertyInfo = typeof(ChangeTracker).GetProperty(nameof(Changes));
        private static readonly PropertyChangedEventArgs ChangesEventArgs = new PropertyChangedEventArgs(nameof(Changes));
        private static readonly ConcurrentDictionary<Type, IReadOnlyList<PropertyInfo>> TrackPropertiesMap = new ConcurrentDictionary<Type, IReadOnlyList<PropertyInfo>>();
        private readonly ItemsChangeTrackers itemsChangeTrackers;
        private readonly PropertiesChangeTrackers propertiesChangeTrackers;

        private int changes;
        private bool disposed;

        public ChangeTracker(INotifyPropertyChanged source, ChangeTrackerSettings settings)
        {
            this.Settings = settings;
            this.propertiesChangeTrackers = PropertiesChangeTrackers.Create(source, this);
            this.itemsChangeTrackers = ItemsChangeTrackers.Create(source, this);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public event EventHandler Changed;

        public ChangeTrackerSettings Settings { get; }

        /// <inheritdoc/>
        public virtual int Changes
        {
            get
            {
                return this.changes;
            }

            protected internal set
            {
                if (value == this.changes)
                {
                    return;
                }

                this.changes = value;
                this.OnPropertyChanged(ChangesEventArgs);
                this.OnChanged();
            }
        }

        /// <summary>
        /// Creates a tracker that detects and notifies about changes of any property or subproperty of <paramref name="root"/>
        /// </summary>
        /// <param name="root">The item to track changes for.</param>
        /// <returns>An <see cref="IValueTracker"/> that signals on changes in <paramref name="root"/></returns>
        public static IChangeTracker Track(INotifyPropertyChanged root)
        {
            Ensure.NotNull(root, nameof(root));
            return Track(root, ChangeTrackerSettings.Default);
        }

        /// <summary>
        /// Creates a tracker that detects and notifies about changes of any property or subproperty of <paramref name="root"/>
        /// </summary>
        /// <param name="root">The item to track changes for.</param>
        /// <param name="settings">Settings telling the tracker which types to ignore.</param>
        /// <returns>An <see cref="IValueTracker"/> that signals on changes in <paramref name="root"/></returns>
        public static IChangeTracker Track(INotifyPropertyChanged root, ChangeTrackerSettings settings)
        {
            Ensure.NotNull(root, nameof(root));
            Ensure.NotNull(settings, nameof(settings));
            return new ChangeTracker(root, settings);
        }

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.propertiesChangeTrackers?.Dispose();
                this.itemsChangeTrackers?.Dispose();
                // Free any other managed objects here.
            }
        }

        protected void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        private void OnChanged()
        {
            this.Changed?.Invoke(this, EventArgs.Empty);
        }

        private sealed class PropertiesChangeTrackers : IDisposable
        {
            private readonly INotifyPropertyChanged source;
            private readonly ChangeTracker parent;
            private readonly PropertyCollection propertyTrackers;

            private PropertiesChangeTrackers(INotifyPropertyChanged source, ChangeTracker parent)
            {
                this.source = source;
                this.parent = parent;
                source.PropertyChanged += this.OnTrackedPropertyChanged;
                List<PropertyCollection.PropertyAndDisposable> items = null;
                foreach (var propertyInfo in source.GetType()
                                              .GetProperties(Constants.DefaultPropertyBindingFlags))
                {
                    if (parent.Settings.IsIgnored(propertyInfo))
                    {
                        continue;
                    }

                    var tracker = this.CreatePropertyTracker(propertyInfo);
                    if (items == null)
                    {
                        items = new List<PropertyCollection.PropertyAndDisposable>();
                    }

                    items.Add(new PropertyCollection.PropertyAndDisposable(propertyInfo, tracker));
                }

                if (items != null)
                {
                    this.propertyTrackers = new PropertyCollection(items);
                }
            }

            internal static PropertiesChangeTrackers Create(
                INotifyPropertyChanged source,
                ChangeTracker parent)
            {
                if (source == null)
                {
                    return null;
                }

                if (parent.Settings.IsIgnored(source.GetType()))
                {
                    return null;
                }

                return new PropertiesChangeTrackers(source, parent);
            }

            public void Dispose()
            {
                this.source.PropertyChanged -= this.OnTrackedPropertyChanged;
                this.propertyTrackers?.Dispose();
            }

            private void OnTrackedPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (string.IsNullOrEmpty(e.PropertyName))
                {
                    this.parent.Changes++;
                    this.Reset();
                    return;
                }

                var propertyInfo = sender.GetType()
                                         .GetProperty(e.PropertyName, Constants.DefaultPropertyBindingFlags);

                if (this.parent.Settings.IsIgnored(propertyInfo))
                {
                    return;
                }

                var propertyTracker = this.CreatePropertyTracker(propertyInfo);
                this.propertyTrackers[propertyInfo] = propertyTracker;
                this.parent.Changes++;
            }

            private void Reset()
            {
                if (this.propertyTrackers == null)
                {
                    return;
                }

                var properties = this.source.GetType().GetProperties(Constants.DefaultPropertyBindingFlags);
                foreach (var propertyInfo in properties)
                {
                    if (this.parent.Settings.IsIgnored(propertyInfo))
                    {
                        continue;
                    }

                    // might be worth it to check if source ReferenceEquals to avoid creating a new tracker here.
                    // Probably not a big problem as I expect PropertyChanged.Invoke(string.Empty) to be rare.
                    this.propertyTrackers[propertyInfo] = this.CreatePropertyTracker(propertyInfo);
                }
            }

            private PropertyChangeTracker CreatePropertyTracker(PropertyInfo propertyInfo)
            {
                var sv = propertyInfo.GetValue(this.source);
                if (sv == null)
                {
                    return null;
                }

                var type = sv.GetType();
                if (type.IsImmutable())
                {
                    return null;
                }

                var notifyPropertyChanged = sv as INotifyPropertyChanged;
                if (notifyPropertyChanged == null)
                {
                    var parentType = this.source.GetType();
                    var message = $"Create tracker failed for {type.FullPrettyName()}.{propertyInfo.Name}.\r\n" +
                                  $"Solve the problem by any of:\r\n" +
                                  $"* Add a specialcase to tracker setting example:\r\n" +
                                  $"    settings.AddSpecialType<{propertyInfo.PropertyType.FullPrettyName()}>(...)\r\n" +
                                  $"    or:" +
                                  $"    settings.AddSpecialProperty(typeof({parentType.PrettyName()}).GetProperty(nameof({parentType.PrettyName()}.{propertyInfo.Name}))" +
                                  $"    Note that this requires you to track changes.\r\n" +
                                  $"* Implement {nameof(INotifyPropertyChanged)} for {parentType.FullPrettyName()}\r\n" +
                                  $"* Implement {nameof(INotifyCollectionChanged)} for {parentType.FullPrettyName()}\r\n" +
                                  $"  Make {parentType.FullPrettyName()} Immutable. Note: To be immutable the class must be sealed.";
                    throw new ArgumentException(message);
                }

                return new PropertyChangeTracker(notifyPropertyChanged, propertyInfo, this.parent);
            }
        }

        private sealed class ItemsChangeTrackers : IDisposable
        {
            private readonly INotifyCollectionChanged source;
            private readonly ChangeTracker parent;
            private readonly ItemCollection<IChangeTracker> itemTrackers;

            private ItemsChangeTrackers(INotifyCollectionChanged source, ChangeTracker parent)
            {
                this.source = source;
                this.parent = parent;
                this.source.CollectionChanged += this.OnTrackedCollectionChanged;
                var sourceList = (IList)source;

                var itemType = sourceList.GetType()
                                         .GetItemType();
                if (!itemType.IsImmutable() && !parent.Settings.IsIgnored(itemType))
                {
                    this.itemTrackers = new ItemCollection<IChangeTracker>();
                    for (int i = 0; i < Math.Max(sourceList.Count, sourceList.Count); i++)
                    {
                        var itemTracker = this.CreateItemTracker(i);
                        this.itemTrackers[i] = itemTracker;
                    }
                }
            }

            internal static ItemsChangeTrackers Create(object source, ChangeTracker parent)
            {
                var xCollectionChanged = source as INotifyCollectionChanged;
                if (xCollectionChanged != null)
                {
                    return new ItemsChangeTrackers(xCollectionChanged, parent);
                }

                return null;
            }

            public void Dispose()
            {
                this.source.CollectionChanged -= this.OnTrackedCollectionChanged;
                this.itemTrackers?.Dispose();
            }

            private void OnTrackedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (this.itemTrackers == null)
                {
                    this.parent.Changes++;
                    return;
                }

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        for (int i = e.NewStartingIndex; i < e.NewStartingIndex + e.NewItems.Count; i++)
                        {
                            var itemTracker = this.CreateItemTracker(i);
                            this.itemTrackers[i] = itemTracker;
                        }

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        for (int i = e.OldStartingIndex; i < e.OldStartingIndex + e.OldItems.Count; i++)
                        {
                            this.itemTrackers.RemoveAt(i);
                        }

                        break;
                    case NotifyCollectionChangedAction.Replace:
                        this.itemTrackers.RemoveAt(e.NewStartingIndex);
                        this.itemTrackers.Insert(e.NewStartingIndex, this.CreateItemTracker(e.NewStartingIndex));
                        break;
                    case NotifyCollectionChangedAction.Move:
                        this.itemTrackers[e.OldStartingIndex] = this.CreateItemTracker(e.OldStartingIndex);
                        this.itemTrackers[e.NewStartingIndex] = this.CreateItemTracker(e.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        {
                            var xList = (IList)this.source;
                            this.itemTrackers.Clear();
                            for (int i = 0; i < xList.Count; i++)
                            {
                                var itemTracker = this.CreateItemTracker(i);
                                this.itemTrackers[i] = itemTracker;
                            }

                            break;
                        }

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.parent.Changes++;
            }

            private ItemChangeTracker CreateItemTracker(int index)
            {
                var sourceList = (IList)this.source;
                var sv = sourceList[index];

                if (sv == null)
                {
                    return null;
                }

                var parentType = sv.GetType();
                if (this.parent.Settings.IsIgnored(parentType))
                {
                    return null;
                }

                var inpc = sv as INotifyPropertyChanged;
                if (inpc == null)
                {
                    //var message = $"Create tracker failed for {parentType.FullPrettyName()}.{propertyInfo.Name}.\r\n"
                    //              + $"Solve the problem by any of:\r\n"
                    //              + $"* Add a specialcase to tracker setting example:\r\n"
                    //              + $"    settings.AddSpecialType<{propertyInfo.PropertyType.FullPrettyName()}>(...)\r\n"
                    //              + $"    or:"
                    //              + $"    settings.AddSpecialProperty(typeof({parentType.PrettyName()}).GetProperty(nameof({parentType.PrettyName()}.{propertyInfo.Name}))"
                    //              + $"    Note that this requires you to track changes.\r\n"
                    //              + $"* Implement {nameof(INotifyPropertyChanged)} for {parentType.FullPrettyName()}\r\n"
                    //              + $"* Implement {nameof(INotifyCollectionChanged)} for {parentType.FullPrettyName()}\r\n"
                    //              + $"  Make {parentType.FullPrettyName()} Immutable. Note: To be immutable the class must be sealed.";
                    throw new ArgumentException("Create item tracker failed");
                }

                return new ItemChangeTracker(inpc, this.parent);
            }
        }
    }
}