namespace Gu.State
{
    using System;
    using System.ComponentModel;

    public static partial class Synchronize
    {
        private sealed class Synchronizer : IDisposable
        {
            private readonly IRefCounted<DirtyTrackerNode> dirtyTrackerNode;

            internal Synchronizer(INotifyPropertyChanged source, INotifyPropertyChanged target, PropertiesSettings settings)
            {
                this.Settings = settings;
                this.dirtyTrackerNode = DirtyTrackerNode.GetOrCreate(source, target, settings, true);
                this.dirtyTrackerNode.Value.Changed += this.OnDirtyTrackerNodeChanged;
                Copy.PropertyValues(source, target, settings);
            }

            public PropertiesSettings Settings { get; }

            public void Dispose()
            {
                this.dirtyTrackerNode.Value.Changed -= this.OnDirtyTrackerNodeChanged;
                this.dirtyTrackerNode.Dispose();
            }

            private void OnDirtyTrackerNodeChanged(object sender, DirtyTrackerChangedEventArgs e)
            {
                var root = e.Root;
                if (!root.Node.IsDirty)
                {
                    return;
                }

                Copy.PropertyValues(root.Node.X, root.Node.Y, this.Settings);
            }

            private void OnTargetChanged(object sender, EventArgs e)
            {
                //if (this.isSynchronizing)
                //{
                //    return;
                //}

                // think we want to track and throw here.
                // this is not perfect
                var message = "Target cannot be modified when a synchronizer is applied to it\r\n" +
                              "The change would just trigger a dirty notification and the value would be updated with the value from source.";
                throw new InvalidOperationException(message);
            }
        }
    }
}