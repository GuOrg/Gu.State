namespace Gu.State
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Tracks changes in a graph.
    /// Listens to nested Property and collection changes.
    /// </summary>
    public sealed class ChangeTracker : IChangeTracker
    {
        private static readonly PropertyChangedEventArgs ChangesEventArgs = new PropertyChangedEventArgs(nameof(Changes));
        private readonly IRefCounted<ChangeTrackerNode> node;
        private bool disposed;

        private int changes;

        public ChangeTracker(INotifyPropertyChanged source, PropertiesSettings settings)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(settings, nameof(settings));
            Track.Verify.CanTrackType(source.GetType(), settings);
            this.Settings = settings;
            this.node = ChangeTrackerNode.GetOrCreate(source, settings, true);
            this.node.Value.Changed += this.OnNodeChange;
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

            private set
            {
                if (value == this.changes)
                {
                    return;
                }

                this.changes = value;
                this.PropertyChanged?.Invoke(this, ChangesEventArgs);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.node.Value.Changed -= this.OnNodeChange;
            this.node.Dispose();
        }

        private void OnNodeChange(object sender, EventArgs e)
        {
            this.Changed?.Invoke(this, e);
            this.Changes++;
        }
    }
}