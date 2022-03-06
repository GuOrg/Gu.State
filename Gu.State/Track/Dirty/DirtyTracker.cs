namespace Gu.State
{
    using System.ComponentModel;

    internal sealed class DirtyTracker : IDirtyTracker
    {
        private static readonly PropertyChangedEventArgs DiffPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Diff));
        private static readonly PropertyChangedEventArgs IsDirtyPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IsDirty));

        private readonly IRefCounted<DirtyTrackerNode> node;
        private bool disposed;

        internal DirtyTracker(INotifyPropertyChanged x, INotifyPropertyChanged y, PropertiesSettings settings)
        {
            this.Settings = settings;
            this.node = DirtyTrackerNode.GetOrCreate(x, y, settings, isRoot: true);
            this.node.Value.PropertyChanged += this.OnNodeChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PropertiesSettings Settings { get; }

        public bool IsDirty => this.node.Value.IsDirty;

        public ValueDiff Diff => this.node.Value.Diff;

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
            else
            {
                throw Throw.ShouldNeverGetHereException($"Expected property name {nameof(DirtyTrackerNode.Diff)} || {nameof(DirtyTrackerNode.IsDirty)}");
            }
        }
    }
}
