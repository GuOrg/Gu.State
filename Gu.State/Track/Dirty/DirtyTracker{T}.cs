namespace Gu.State
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    /// <summary>
    /// Tracks if there is a difference between the properties of x and y
    /// </summary>
    /// <typeparam name="T">
    /// The type to synchronize must implement <see cref="INotifyPropertyChanged"/>
    /// Collection must implement <see cref="System.Collections.Specialized.INotifyCollectionChanged"/>
    /// All types in the graph muct be either notifying or immutable.
    /// </typeparam>
    [DebuggerDisplay("DirtyTracker<{typeof(T).Name}> IsDirty: {IsDirty}")]
    public sealed class DirtyTracker<T> : DirtyTracker, INotifyPropertyChanged, IDisposable
        where T : class, INotifyPropertyChanged
    {
        private readonly IRefCounted<DirtyTrackerNode> node;
        private bool disposed;

        public DirtyTracker(T x, T y, PropertiesSettings settings)
        {
            Ensure.NotNull(x, nameof(x));
            Ensure.NotNull(y, nameof(y));
            Ensure.NotSame(x, y, nameof(x), nameof(y));
            Ensure.SameType(x, y);
            Track.VerifyCanTrackIsDirty<T>(settings);
            this.Settings = settings;
            this.node = DirtyTrackerNode.GetOrCreate(x, y, settings, true);
            this.node.Value.PropertyChanged += this.OnNodeChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override bool IsDirty => this.node.Value.IsDirty;

        internal override ValueDiff Diff => this.node.Value.Diff;

        public PropertiesSettings Settings { get; }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.node.Value.PropertyChanged -= this.OnNodeChanged;
            this.node.Dispose();
        }

        private void OnNodeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DirtyTrackerNode.Diff))
            {
                this.PropertyChanged?.Invoke(this, DiffPropertyChangedEventArgs);
            }
            else if (e.PropertyName == nameof(DirtyTrackerNode.IsDirty))
            {
                this.PropertyChanged?.Invoke(this, IsDirtyPropertyChangedEventArgs);
            }
        }
    }
}
