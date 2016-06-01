namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;

    public static partial class Synchronize
    {
        private sealed class Synchronizer : IDisposable
        {
            private readonly IRefCounted<DirtyTrackerNode> dirtyTrackerNode;
            private readonly IBorrowed<ConcurrentQueue<DirtyTrackerNode>> borrowedQueue;
            private readonly object gate = new object();
            private DirtyTrackerNode processingNode;

            internal Synchronizer(INotifyPropertyChanged source, INotifyPropertyChanged target, PropertiesSettings settings)
            {
                this.Settings = settings;
                this.dirtyTrackerNode = DirtyTrackerNode.GetOrCreate(source, target, settings, true);
                this.dirtyTrackerNode.Value.Changed += this.OnDirtyTrackerNodeChanged;
                this.borrowedQueue = ConcurrentQueuePool<DirtyTrackerNode>.Borrow();
                this.AddToSyncQueue(this.dirtyTrackerNode.Value);
            }

            public PropertiesSettings Settings { get; }

            public DirtyTrackerNode RootNode => this.dirtyTrackerNode.Value;

            public void Dispose()
            {
                this.dirtyTrackerNode.Value.Changed -= this.OnDirtyTrackerNodeChanged;
                this.dirtyTrackerNode.Dispose();
                this.borrowedQueue.Dispose();
            }

            private void OnDirtyTrackerNodeChanged(object sender, TrackerChangedEventArgs<DirtyTrackerNode> e)
            {
                var root = e.Root;

                // below is not perfect but catches simple cases of when target changes
                if (this.processingNode == null &&
                    ReferenceEquals(root.Node.Y, root.EventArgs.Source) && 
                    !ReferenceEquals(root.Node.X, root.EventArgs.Source))
                {
                    var message = "Target cannot be modified when a synchronizer is applied to it\r\n" +
                                  "The change would just trigger a dirty notification and the value would be updated with the value from source.";
                    throw new InvalidOperationException(message);
                }

                if (!root.Node.IsDirty)
                {
                    return;
                }

                this.AddToSyncQueue(root.Node);
            }

            private void AddToSyncQueue(DirtyTrackerNode newNode)
            {
                var queue = this.borrowedQueue.Value;
                queue.Enqueue(newNode);
                if (this.processingNode != null)
                {
                    return;
                }

                lock (this.gate)
                {
                    if (this.processingNode != null)
                    {
                        return;
                    }

                    while (queue.TryDequeue(out this.processingNode))
                    {
                        if (this.processingNode.IsDirty)
                        {
                            Copy.PropertyValues(this.processingNode.X, this.processingNode.Y, this.Settings);
                        }
                    }

                    this.processingNode = null;
                }
            }
        }
    }
}