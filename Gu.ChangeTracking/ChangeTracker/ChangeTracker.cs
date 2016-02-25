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
    using System.Runtime.CompilerServices;
    using System.Text;
    using JetBrains.Annotations;

    /// <summary>
    /// Tracks changes in a graph.
    /// Listens to nested Property and collection changes.
    /// </summary>
    public class ChangeTracker : IChangeTracker
    {
        private static readonly PropertyChangedEventArgs ChangesEventArgs = new PropertyChangedEventArgs(nameof(Changes));
        private readonly ItemsChangeTrackers itemsChangeTrackers;
        private readonly PropertiesChangeTrackers propertiesChangeTrackers;

        private int changes;
        private bool disposed;

        internal ChangeTracker(INotifyPropertyChanged source, ChangeTrackerSettings settings, PropertyPath path)
        {
            this.Source = source;
            this.Settings = settings;
            this.Path = path;
            Ensure.IsTrackableType(source.GetType(), this);
            this.propertiesChangeTrackers = PropertiesChangeTrackers.Create(source, this);
            this.itemsChangeTrackers = ItemsChangeTrackers.Create(source, this);
            if (this.propertiesChangeTrackers == null && this.itemsChangeTrackers == null)
            {
                throw new InvalidOperationException("Created a tracker that does not track anything");
            }
        }

        private ChangeTracker(INotifyPropertyChanged source, ChangeTrackerSettings settings)
            : this(source, settings, new PropertyPath(source.GetType()))
        {
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

        internal object Source { get; }

        internal PropertyPath Path { get; }

        /// <summary>
        /// Creates a tracker that detects and notifies about changes of any property or subproperty of <paramref name="root"/>
        /// </summary>
        /// <param name="root">The item to track changes for.</param>
        /// <returns>An <see cref="IValueTracker"/> that signals on changes in <paramref name="root"/></returns>
        public static IChangeTracker Track(INotifyPropertyChanged root)
        {
            ChangeTracking.Ensure.NotNull(root, nameof(root));
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
                if (parent.Settings.IsIgnored(sourceType))
                {
                    return null;
                }

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

            internal static IEnumerable<PropertyInfo> GetTrackProperties(Type sourceType, ChangeTrackerSettings settings)
            {
                return sourceType.GetProperties(Constants.DefaultPropertyBindingFlags)
                                 .Where(p => IsTrackProperty(p, settings));
            }

            internal static bool IsTrackProperty(PropertyInfo propertyInfo, ChangeTrackerSettings settings)
            {
                if (settings.IsIgnored(propertyInfo))
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

                Ensure.IsTrackableType(sv.GetType(), parent);
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

                if (this.parent.Settings.IsIgnored(propertyInfo))
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

                Ensure.IsTrackableType(source.GetType(), parent);

                var incc = source as INotifyCollectionChanged;
                var itemType = source.GetType().GetItemType();
                if (itemType.IsImmutable() || parent.Settings.IsIgnored(itemType))
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

                if (parent.Settings.IsIgnored(itemType))
                {
                    return null;
                }

                Ensure.IsTrackableType(itemType, parent);
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
                        throw new ArgumentOutOfRangeException();
                }

                this.parent.Changes++;
            }
        }

        /// <summary>
        /// This class is used for failing fast and throwing with nice exception messages.
        /// </summary>
        private static class Ensure
        {
            private static readonly ConditionalWeakTable<ChangeTrackerSettings, HashSet<Type>> Cache = new ConditionalWeakTable<ChangeTrackerSettings, HashSet<Type>>();

            internal static void IsTrackableType(Type type, ChangeTracker tracker)
            {
                var checkedTypes = Cache.GetOrCreateValue(tracker.Settings);
                if (checkedTypes.Contains(type))
                {
                    return;
                }

                var path = tracker.Path;
                CheckProperties(type, path, tracker.Settings);
                VerifyItems(type, path, tracker.Settings);
                checkedTypes.Add(type);
            }

            private static void IsTrackableType(Type type, PropertyPath path, ChangeTrackerSettings settings)
            {
                var checkedTypes = Cache.GetOrCreateValue(settings);
                if (checkedTypes.Contains(type))
                {
                    return;
                }

                CheckProperties(type, path, settings);
                VerifyItems(type, path, settings);
                checkedTypes.Add(type);
            }

            private static void IsTrackableIfEnumerable(Type type, PropertyPath propertyPath)
            {
                if (!typeof(IEnumerable).IsAssignableFrom(type))
                {
                    return;
                }

                if (!typeof(INotifyCollectionChanged).IsAssignableFrom(type) || !typeof(INotifyCollectionChanged).IsAssignableFrom(type))
                {
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendCreateFailedForLine<ChangeTracker>(propertyPath)
                                  .AppendSolveTheProblemByLine()
                                  .AppendSuggestionsForEnumerableLines(type, propertyPath)
                                  .AppendUseImmutableTypeLine(propertyPath)
                                  .AppendChangeTrackerSettingsSpecialCaseLines(type, propertyPath);

                    var message = messageBuilder.ToString();
                    throw new NotSupportedException(message);
                }
            }

            internal static void IsPropertyChanged(Type type, PropertyPath propertyPath)
            {
                if (!typeof(INotifyPropertyChanged).IsAssignableFrom(type))
                {
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendCreateFailedForLine<ChangeTracker>(propertyPath)
                                  .AppendSolveTheProblemByLine()
                                  .AppendImplementsLine<INotifyPropertyChanged>(type, propertyPath)
                                  .AppendUseImmutableTypeLine(propertyPath)
                                  .AppendChangeTrackerSettingsSpecialCaseLines(type, propertyPath);

                    var message = messageBuilder.ToString();
                    throw new NotSupportedException(message);
                }
            }

            private static void CheckProperties(Type type, PropertyPath path, ChangeTrackerSettings settings)
            {
                var properties = PropertiesChangeTrackers.GetTrackProperties(type, settings)
                                         .ToArray();
                foreach (var propertyInfo in properties)
                {
                    if (path.Path.OfType<PropertyItem>().Any(p => p.Property.PropertyType == type))
                    {
                        // stopping recursion if a type has self type as property
                        continue;
                    }

                    var propertyPath = path.WithProperty(propertyInfo);
                    if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        IsTrackableIfEnumerable(propertyInfo.PropertyType, propertyPath);
                    }
                    else if (!settings.IsIgnored(propertyInfo.PropertyType))
                    {
                        IsPropertyChanged(propertyInfo.PropertyType, propertyPath);
                    }

                    IsTrackableType(propertyInfo.PropertyType, propertyPath, settings);
                }
            }

            private static void VerifyItems(Type type, PropertyPath path, ChangeTrackerSettings settings)
            {
                if (!typeof(IEnumerable).IsAssignableFrom(type))
                {
                    return;
                }

                IsTrackableIfEnumerable(type, path);
                var itemType = type.GetItemType();
                var propertyPath = path.WithIndexer();
                IsTrackableType(itemType, propertyPath, settings);
            }
        }
    }
}