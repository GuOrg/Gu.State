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
    public abstract class ChangeTracker : ITracker
    {
        private static readonly PropertyInfo ChangesPropertyInfo = typeof(ChangeTracker).GetProperty(nameof(Changes));
        private static readonly PropertyChangedEventArgs ChangesEventArgs = new PropertyChangedEventArgs(nameof(Changes));
        private static readonly ConcurrentDictionary<Type, IReadOnlyList<PropertyInfo>> TrackPropertiesMap = new ConcurrentDictionary<Type, IReadOnlyList<PropertyInfo>>();
        private int changes;
        private bool disposed;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public event EventHandler Changed;

        /// <inheritdoc/>
        public int Changes
        {
            get
            {
                return this.changes;
            }

            protected set
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
        public static IValueTracker Track(INotifyPropertyChanged root)
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
        public static IValueTracker Track(INotifyPropertyChanged root, ChangeTrackerSettings settings)
        {
            Ensure.NotNull(root, nameof(root));
            Ensure.NotNull(settings, nameof(settings));

            var tracker = Create(typeof(ChangeTracker), ChangesPropertyInfo, root, settings);
            Ensure.NotNull(tracker, nameof(tracker));
            return tracker;
        }

        private static void Verify(Type parentType, PropertyInfo parentProperty, object value, ChangeTrackerSettings settings)
        {
            if (value is IEnumerable && !(value is INotifyCollectionChanged))
            {
                // goto throw :)
            }
            else
            {
                if (settings.IsIgnored(parentProperty))
                {
                    return;
                }

                var propertyType = parentProperty.PropertyType;
                if (settings.IsIgnored(propertyType))
                {
                    return;
                }

                if (!IsTrackType(propertyType, settings))
                {
                    return;
                }

                if (typeof(INotifyPropertyChanged).IsAssignableFrom(propertyType))
                {
                    return;
                }

                if (typeof(INotifyCollectionChanged).IsAssignableFrom(propertyType))
                {
                    return;
                }
            }

            // settings.AddSpecialType<FileInfo>(TrackAs.Explicit)
            var message = $"Create tracker failed for {parentType.FullPrettyName()}.{parentProperty.Name}.\r\n" +
                          $"Solve the problem by any of:\r\n" +
                          $"* Add a specialcase to tracker setting example:\r\n" +
                          $"    settings.AddSpecialType<{parentProperty.PropertyType.FullPrettyName()}>(...)\r\n" +
                          $"    or:" +
                          $"    settings.AddSpecialProperty(typeof({parentType.PrettyName()}).GetProperty(nameof({parentType.PrettyName()}.{parentProperty.Name}))" +
                          $"    Note that this requires you to track changes.\r\n" +
                          $"* Implement {nameof(INotifyPropertyChanged)} for {parentType.FullPrettyName()}\r\n" +
                          $"* Implement {nameof(INotifyCollectionChanged)} for {parentType.FullPrettyName()}\r\n";
            throw new ArgumentException(message);
        }

        internal static bool CanTrack(Type parentType, PropertyInfo parentProperty, object value, ChangeTrackerSettings settings)
        {
            Verify(parentType, parentProperty, value, settings);
            if (settings.IsIgnored(parentProperty))
            {
                return false;
            }

            if (settings.IsIgnored(parentType))
            {
                return false;
            }

            var incc = value as INotifyCollectionChanged;
            if (incc != null)
            {
                return true;
            }

            var inpc = value as INotifyPropertyChanged;
            if (inpc != null)
            {
                return true;
            }

            return false;
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

        protected static IPropertyTracker Create(Type parentType, PropertyInfo parentProperty, object child, ChangeTrackerSettings settings)
        {
            if (!CanTrack(parentType, parentProperty, child, settings))
            {
                return null;
            }

            if (child == null)
            {
                return null;
            }

            var incc = child as INotifyCollectionChanged;
            if (incc != null)
            {
                return new CollectionTracker(parentType, parentProperty, (IEnumerable)incc, settings);
            }

            var inpc = child as INotifyPropertyChanged;
            if (inpc != null)
            {
                return new PropertyChangeTracker(parentType, parentProperty, inpc, settings);
            }

            throw new ArgumentOutOfRangeException($"Could not create a tracker for {child}");
        }

        protected static IReadOnlyList<PropertyInfo> GetTrackProperties(INotifyPropertyChanged item, ChangeTrackerSettings settings)
        {
            if (item == null)
            {
                return null;
            }

            var trackProperties = TrackPropertiesMap.GetOrAdd(item.GetType(), t => TrackPropertiesFor(t, settings));
            return trackProperties;
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
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

        protected static bool IsTrackType(Type type, ChangeTrackerSettings settings)
        {
            Ensure.NotNull(type, nameof(type));
            if (type == typeof(string) || type.IsEnum || type.IsPrimitive)
            {
                return false;
            }

            if (settings.IsIgnored(type))
            {
                return false;
            }

            return true;
        }

        private static IReadOnlyList<PropertyInfo> TrackPropertiesFor(Type type, ChangeTrackerSettings settings)
        {
            var propertyInfos = type.GetProperties()
                                    .Where(x => x.GetIndexParameters().Length == 0 && IsTrackType(x.PropertyType, settings))
                                    .ToArray();
            return propertyInfos;
        }
    }
}