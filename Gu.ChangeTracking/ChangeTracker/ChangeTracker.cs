namespace Gu.ChangeTracking
{
    using System;
    using System.ComponentModel;

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
                throw ChangeTracking.Throw.ThrowThereIsABugInTheLibrary("Created a tracker that does not track anything");
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
        /// <param name="referenceHandling">
        /// </param>
        /// <returns>An <see cref="IValueTracker"/> that signals on changes in <paramref name="root"/></returns>
        public static IChangeTracker Track(INotifyPropertyChanged root, ReferenceHandling referenceHandling = ReferenceHandling.Structural)
        {
            Ensure.NotNull(root, nameof(root));
            var settings = PropertiesSettings.GetOrCreate(referenceHandling: referenceHandling);
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
    }
}