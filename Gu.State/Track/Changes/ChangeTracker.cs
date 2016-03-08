namespace Gu.State
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Tracks changes in a graph.
    /// Listens to nested property and collection changes.
    /// </summary>
    public partial class ChangeTracker : IChangeTracker, ITracker
    {
        private static readonly PropertyChangedEventArgs ChangesEventArgs = new PropertyChangedEventArgs(nameof(Changes));

        private int changes;
        private bool disposed;

        private INode<ItemReference, ITracker> node;

        public ChangeTracker(INotifyPropertyChanged source, PropertiesSettings settings)
            : this(source, settings, null)
        {
        }

        internal ChangeTracker(INotifyPropertyChanged source, PropertiesSettings settings, INode<ItemReference, ITracker> node)
        {
            this.Settings = settings;
            Track.Verify.IsTrackableType(source.GetType(), this);
            this.node = node ?? TrackerNode.CreateRoot(new ItemReference(source), (ITracker)this);
            this.node.AddChild(new ItemReference(source, "Properties"), () => PropertiesChangeTrackers.Create(source, settings, this.node));
            //this.node.AddChild(new ItemReference(source, "Items"), () => ItemsChangeTrackers.Create(source, settings));

            throw new NotImplementedException("Check referencehandling, dont create tree if referecnes");
            throw new NotImplementedException("Assert that something is tracked");
            //if (this.propertiesChangeTrackers == null && this.itemsChangeTrackers == null)
            //{
            //    throw State.Throw.ThrowThereIsABugInTheLibrary("Created a tracker that does not track anything");
            //}
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public event EventHandler Changed;

        void ITracker.ChildChanged(ITracker child)
        {
            this.Changes++;
        }

        public PropertiesSettings Settings { get; }

        /// <inheritdoc/>
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
                this.OnPropertyChanged(ChangesEventArgs);
                this.OnChanged();
            }
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
                this.node.Dispose();
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