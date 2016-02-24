namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
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

        internal object Source => (object)this.propertiesChangeTrackers?.Source ?? this.itemsChangeTrackers.Source;

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
            internal readonly INotifyPropertyChanged Source;
            private readonly ChangeTracker parent;
            private readonly PropertyCollection propertyTrackers;

            private PropertiesChangeTrackers(INotifyPropertyChanged source, ChangeTracker parent, PropertyCollection propertyTrackers)
            {
                this.Source = source;
                this.parent = parent;
                this.propertyTrackers = propertyTrackers;
                source.PropertyChanged += this.OnTrackedPropertyChanged;
            }

            public void Dispose()
            {
                this.Source.PropertyChanged -= this.OnTrackedPropertyChanged;
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
                var properties = sourceType.GetProperties(Constants.DefaultPropertyBindingFlags);
                foreach (var propertyInfo in properties)
                {
                    if (parent.Settings.IsIgnored(propertyInfo))
                    {
                        continue;
                    }

                    var tracker = CreatePropertyTracker(source, propertyInfo, parent);
                    if (items == null)
                    {
                        items = new List<PropertyCollection.PropertyAndDisposable>();
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

                var propertyTracker = CreatePropertyTracker(this.Source, propertyInfo, this.parent);
                this.propertyTrackers[propertyInfo] = propertyTracker;
                this.parent.Changes++;
            }

            private void Reset()
            {
                if (this.propertyTrackers == null)
                {
                    return;
                }

                var properties = this.Source.GetType()
                                     .GetProperties(Constants.DefaultPropertyBindingFlags);
                foreach (var propertyInfo in properties)
                {
                    if (this.parent.Settings.IsIgnored(propertyInfo))
                    {
                        continue;
                    }

                    // might be worth it to check if Source ReferenceEquals to avoid creating a new tracker here.
                    // Probably not a big problem as I expect PropertyChanged.Invoke(string.Empty) to be rare.
                    this.propertyTrackers[propertyInfo] = CreatePropertyTracker(this.Source, propertyInfo, this.parent);
                }
            }

            private static PropertyChangeTracker CreatePropertyTracker(object source, PropertyInfo propertyInfo, ChangeTracker parent)
            {
                var sv = propertyInfo.GetValue(source);
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
                    var parentType = source?.GetType() ?? propertyInfo.DeclaringType;
                    var message = $"Create tracker failed for {type.FullPrettyName()}.{propertyInfo.Name}.\r\n"
                                  + $"Solve the problem by any of:\r\n"
                                  + $"* Add a specialcase to tracker setting example:\r\n"
                                  + $"    settings.AddSpecialType<{propertyInfo.PropertyType.FullPrettyName()}>(...)\r\n"
                                  + $"    or:"
                                  + $"    settings.AddSpecialProperty(typeof({parentType.PrettyName()}).GetProperty(nameof({parentType.PrettyName()}.{propertyInfo.Name}))"
                                  + $"    Note that this requires you to track changes.\r\n"
                                  + $"* Implement {nameof(INotifyPropertyChanged)} for {parentType.FullPrettyName()}\r\n"
                                  + $"* Implement {nameof(INotifyCollectionChanged)} for {parentType.FullPrettyName()}\r\n"
                                  + $"  Make {parentType.FullPrettyName()} Immutable. Note: To be immutable the class must be sealed.";
                    throw new ArgumentException(message);
                }

                return new PropertyChangeTracker(notifyPropertyChanged, propertyInfo, parent);
            }
        }

        private sealed class ItemsChangeTrackers : IDisposable
        {
            internal readonly INotifyCollectionChanged Source;
            private readonly ChangeTracker parent;
            private readonly ItemCollection<ChangeTracker> itemTrackers;

            private ItemsChangeTrackers(INotifyCollectionChanged source, ChangeTracker parent, ItemCollection<ChangeTracker> itemTrackers)
            {
                this.Source = source;
                this.parent = parent;
                this.itemTrackers = itemTrackers;
                this.Source.CollectionChanged += this.OnTrackedCollectionChanged;
            }

            internal static ItemsChangeTrackers Create(object source, ChangeTracker parent)
            {
                if (!(source is IEnumerable))
                {
                    return null;
                }

                if (!(source is INotifyCollectionChanged) || !(source is IList))
                {
                    var propertyPath = PropertyPath.Create(parent);
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendCreateFailedForLine<ChangeTracker>(propertyPath)
                                  .AppendSolveTheProblemByLine()
                                  .AppendImplementsIListAndINotifyCollectionChangedLines(source, propertyPath)
                                  .AppendUseImmutableTypeLine(propertyPath)
                                  .AppendChangeTrackerSettingsSpecialCaseLines(source, propertyPath);

                    throw new ArgumentException(messageBuilder.ToString(), nameof(source));
                }

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

            public void Dispose()
            {
                this.Source.CollectionChanged -= this.OnTrackedCollectionChanged;
                this.itemTrackers?.Dispose();
            }

            private void OnTrackedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (this.itemTrackers == null)
                {
                    this.parent.Changes++;
                    return;
                }

                var sourceList = (IList)this.Source;
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
                            var xList = (IList)this.Source;
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

            private static ItemChangeTracker CreateItemTracker(IList source, int index, ChangeTracker parent)
            {
                var sv = source[index];

                if (sv == null)
                {
                    return null;
                }

                var parentType = sv.GetType();
                if (parent.Settings.IsIgnored(parentType))
                {
                    return null;
                }

                var inpc = sv as INotifyPropertyChanged;
                if (inpc == null)
                {
                    var propertyPath = PropertyPath.Create(parent);
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendCreateFailedForLine<ChangeTracker>(propertyPath)
                                  .AppendSolveTheProblemByLine()
                                  .AppendImplementsLine<INotifyPropertyChanged>(sv, propertyPath)
                                  .AppendUseImmutableTypeLine(propertyPath)
                                  .AppendChangeTrackerSettingsSpecialCaseLines(sv, propertyPath);

                    throw new ArgumentException(messageBuilder.ToString(), nameof(index));
                }

                return new ItemChangeTracker(inpc, index, parent);
            }
        }

        private static class Validator
        {
            internal static void VerifyCreatePropertyTracker(PropertyPath path)
            {
            }

            internal static void VerifyCreateItemTracker(Type sourceType, PropertyPath path)
            {
                //var type = sourceType ?? path.Path.OfType<PropertyPath.PropertyItem>()
                //                                    .LastOrDefault()
                //                                    ?.GetType()
                //                             ?? path.Root.Type;
                //var itemType = type.GetItemType();
                //if (!itemType.im)

            }
        }
    }
}