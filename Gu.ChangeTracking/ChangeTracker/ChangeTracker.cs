namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Tracks changes in a graph.
    /// Listens to nested Property and collection changes.
    /// </summary>
    public partial class ChangeTracker : IChangeTracker
    {
        private static readonly PropertyChangedEventArgs ChangesEventArgs = new PropertyChangedEventArgs(nameof(Changes));
        private readonly ItemsChangeTrackers itemsChangeTrackers;
        private readonly PropertiesChangeTrackers propertiesChangeTrackers;

        private int changes;
        private bool disposed;

        internal ChangeTracker(INotifyPropertyChanged source, PropertiesSettings settings, MemberPath path)
        {
            this.Settings = settings;
            this.Path = path;
            Verify.IsTrackableType(source.GetType(), this);
            this.propertiesChangeTrackers = PropertiesChangeTrackers.Create(source, this);
            this.itemsChangeTrackers = ItemsChangeTrackers.Create(source, this);
            if (this.propertiesChangeTrackers == null && this.itemsChangeTrackers == null)
            {
                throw Gu.ChangeTracking.Throw.ThrowThereIsABugInTheLibrary("Created a tracker that does not track anything");
            }
        }

        private ChangeTracker(INotifyPropertyChanged source, PropertiesSettings settings)
            : this(source, settings, new MemberPath(source.GetType()))
        {
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public event EventHandler Changed;

        public PropertiesSettings Settings { get; }

        /// <inheritdoc/>
        public int Changes
        {
            get
            {
                return this.changes;
            }
            internal set
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

        internal MemberPath Path { get; }

        /// <summary>
        /// Creates a tracker that detects and notifies about changes of any property or subproperty of <paramref name="root"/>
        /// </summary>
        /// <param name="root">The item to track changes for.</param>
        /// <returns>An <see cref="IValueTracker"/> that signals on changes in <paramref name="root"/></returns>
        public static IChangeTracker Track(INotifyPropertyChanged root)
        {
            ChangeTracking.Ensure.NotNull(root, nameof(root));
            var settings = PropertiesSettings.GetOrCreate();
            return Track(root, settings);
        }

        /// <summary>
        /// Creates a tracker that detects and notifies about changes of any property or subproperty of <paramref name="root"/>
        /// </summary>
        /// <param name="root">The item to track changes for.</param>
        /// <param name="settings">Settings telling the tracker which types to ignore.</param>
        /// <returns>An <see cref="IValueTracker"/> that signals on changes in <paramref name="root"/></returns>
        public static IChangeTracker Track(INotifyPropertyChanged root, PropertiesSettings settings)
        {
            ChangeTracking.Ensure.NotNull(root, nameof(root));
            ChangeTracking.Ensure.NotNull(settings, nameof(settings));
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.propertiesChangeTrackers?.Dispose();
                this.itemsChangeTrackers?.Dispose();
            }
        }

        protected void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
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

            private PropertiesChangeTrackers(INotifyPropertyChanged source, ChangeTracker parent, PropertyCollection propertyTrackers)
            {
                this.source = source;
                this.parent = parent;
                this.propertyTrackers = propertyTrackers;
                source.PropertyChanged += this.OnTrackedPropertyChanged;
            }

            public void Dispose()
            {
                this.source.PropertyChanged -= this.OnTrackedPropertyChanged;
                this.propertyTrackers?.Dispose();
            }

            internal static PropertiesChangeTrackers Create(INotifyPropertyChanged source, ChangeTracker parent)
            {
                if (source == null)
                {
                    return null;
                }

                var sourceType = source.GetType();
                if (parent.Settings.IsIgnoringDeclaringType(sourceType))
                {
                    return null;
                }

                Verify.IsTrackableType(source.GetType(), parent);
                List<PropertyCollection.PropertyAndDisposable> items = null;
                foreach (var propertyInfo in GetTrackProperties(sourceType, parent.Settings))
                {
                    var tracker = CreatePropertyTracker(source, propertyInfo, parent);
                    if (items == null)
                    {
                        items = new List<PropertyCollection.PropertyAndDisposable>(sourceType.GetProperties().Length);
                    }

                    items.Add(new PropertyCollection.PropertyAndDisposable(propertyInfo, tracker));
                }

                if (items != null)
                {
                    var propertyCollection = new PropertyCollection(items);
                    return new PropertiesChangeTrackers(source, parent, propertyCollection);
                }

                return new PropertiesChangeTrackers(source, parent, null);
            }

            internal static IEnumerable<PropertyInfo> GetTrackProperties(Type sourceType, IIgnoringProperties settings)
            {
                return sourceType.GetProperties(Constants.DefaultPropertyBindingFlags)
                                 .Where(p => IsTrackProperty(p, settings));
            }

            private static bool IsTrackProperty(PropertyInfo propertyInfo, IIgnoringProperties settings)
            {
                if (settings.IsIgnoringProperty(propertyInfo))
                {
                    return false;
                }

                if (propertyInfo.PropertyType.IsImmutable())
                {
                    return false;
                }

                return true;
            }

            private static PropertyChangeTracker CreatePropertyTracker(object source, PropertyInfo propertyInfo, ChangeTracker parent)
            {
                if (!IsTrackProperty(propertyInfo, parent.Settings))
                {
                    return null;
                }

                var sv = propertyInfo.GetValue(source);
                if (sv == null)
                {
                    return null;
                }

                Verify.IsTrackablePropertyValue(sv.GetType(), propertyInfo, parent);
                var notifyPropertyChanged = sv as INotifyPropertyChanged;
                return new PropertyChangeTracker(notifyPropertyChanged, propertyInfo, parent);
            }

            private void OnTrackedPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (string.IsNullOrEmpty(e.PropertyName))
                {
                    this.parent.Changes++;
                    this.Reset();
                    return;
                }

                var propertyInfo = sender.GetType().GetProperty(e.PropertyName, Constants.DefaultPropertyBindingFlags);

                if (this.parent.Settings.IsIgnoringProperty(propertyInfo))
                {
                    return;
                }

                if (IsTrackProperty(propertyInfo, this.parent.Settings))
                {
                    var propertyTracker = CreatePropertyTracker(this.source, propertyInfo, this.parent);
                    this.propertyTrackers[propertyInfo] = propertyTracker;
                }

                this.parent.Changes++;
            }

            private void Reset()
            {
                if (this.propertyTrackers == null)
                {
                    return;
                }

                foreach (var propertyInfo in GetTrackProperties(this.source?.GetType(), this.parent.Settings))
                {
                    // might be worth it to check if Source ReferenceEquals to avoid creating a new tracker here.
                    // Probably not a big problem as I expect PropertyChanged.Invoke(string.Empty) to be rare.
                    this.propertyTrackers[propertyInfo] = CreatePropertyTracker(this.source, propertyInfo, this.parent);
                }
            }
        }

        private sealed class ItemsChangeTrackers : IDisposable
        {
            private readonly INotifyCollectionChanged source;
            private readonly ChangeTracker parent;
            private readonly ItemCollection<ChangeTracker> itemTrackers;

            private ItemsChangeTrackers(INotifyCollectionChanged source, ChangeTracker parent, ItemCollection<ChangeTracker> itemTrackers)
            {
                this.source = source;
                this.parent = parent;
                this.itemTrackers = itemTrackers;
                this.source.CollectionChanged += this.OnTrackedCollectionChanged;
            }

            public void Dispose()
            {
                this.source.CollectionChanged -= this.OnTrackedCollectionChanged;
                this.itemTrackers?.Dispose();
            }

            internal static ItemsChangeTrackers Create(object source, ChangeTracker parent)
            {
                if (!(source is IEnumerable))
                {
                    return null;
                }

                var sourceType = source.GetType();
                //if (parent.Settings.IsIgnoringDeclaringType(sourceType))
                //{
                //    return null;
                //}

                Verify.IsTrackableItemValue(sourceType, null, parent);

                var incc = source as INotifyCollectionChanged;
                var itemType = source.GetType().GetItemType();
                if (itemType.IsImmutable() || parent.Settings.IsIgnoringDeclaringType(itemType))
                {
                    return new ItemsChangeTrackers(incc, parent, null);
                }

                var sourceList = (IList)source;
                var itemTrackers = new ItemCollection<ChangeTracker>(sourceList.Count);
                for (int i = 0; i < sourceList.Count; i++)
                {
                    var itemTracker = CreateItemTracker(sourceList, i, parent);
                    itemTrackers[i] = itemTracker;
                }

                return new ItemsChangeTrackers(incc, parent, itemTrackers);
            }

            private static ItemChangeTracker CreateItemTracker(IList source, int index, ChangeTracker parent)
            {
                Debug.Assert(!source.GetType().GetItemType().IsImmutable(), "Creating a tracker for an immutable type would be wasteful");
                var sv = source[index];
                if (sv == null)
                {
                    return null;
                }

                var itemType = sv.GetType();

                if (parent.Settings.IsIgnoringDeclaringType(itemType))
                {
                    return null;
                }

                Verify.IsTrackableItemValue(itemType, index, parent);
                var inpc = sv as INotifyPropertyChanged;
                return new ItemChangeTracker(inpc, index, parent);
            }

            private void OnTrackedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (this.itemTrackers == null)
                {
                    this.parent.Changes++;
                    return;
                }

                var sourceList = (IList)this.source;
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        for (int i = e.NewStartingIndex; i < e.NewStartingIndex + e.NewItems.Count; i++)
                        {
                            var itemTracker = CreateItemTracker(sourceList, i, this.parent);
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
                        this.itemTrackers.Insert(e.NewStartingIndex, CreateItemTracker(sourceList, e.NewStartingIndex, this.parent));
                        break;
                    case NotifyCollectionChangedAction.Move:
                        this.itemTrackers[e.OldStartingIndex] = CreateItemTracker(sourceList, e.OldStartingIndex, this.parent);
                        this.itemTrackers[e.NewStartingIndex] = CreateItemTracker(sourceList, e.NewStartingIndex, this.parent);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        {
                            var xList = (IList)this.source;
                            this.itemTrackers.Clear();
                            for (int i = 0; i < xList.Count; i++)
                            {
                                var itemTracker = CreateItemTracker(sourceList, i, this.parent);
                                this.itemTrackers[i] = itemTracker;
                            }

                            break;
                        }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(e.Action));
                }

                this.parent.Changes++;
            }
        }
    }
}