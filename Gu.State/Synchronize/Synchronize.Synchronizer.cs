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
            private readonly IUnsubscriber<IRefCounted<ChangeTrackerNode>> targetSubscription;
            private readonly object gate = new object();
            private bool isProcessingQueue;

            internal Synchronizer(INotifyPropertyChanged source, INotifyPropertyChanged target, PropertiesSettings settings)
            {
                this.Settings = settings;
                this.dirtyTrackerNode = DirtyTrackerNode.GetOrCreate(source, target, settings, true);
                this.dirtyTrackerNode.Value.Changed += this.OnDirtyTrackerNodeChanged;
                var targetTracker = ChangeTrackerNode.GetOrCreate(target, settings, true);
                targetTracker.Value.Changed += this.OnTargetChanged;
                this.targetSubscription = targetTracker.UnsubscribeAndDispose(x => x.Value.Changed -= this.OnTargetChanged);
                this.borrowedQueue = ConcurrentQueuePool<DirtyTrackerNode>.Borrow();
                this.AddToSyncQueue(this.dirtyTrackerNode.Value);
            }

            public PropertiesSettings Settings { get; }

            public void Dispose()
            {
                this.dirtyTrackerNode.Value.Changed -= this.OnDirtyTrackerNodeChanged;
                this.dirtyTrackerNode.Dispose();
                this.borrowedQueue.Dispose();
                this.targetSubscription.Dispose();
            }

            private void OnDirtyTrackerNodeChanged(object sender, TrackerChangedEventArgs<DirtyTrackerNode> e)
            {
                var root = e.Root;
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
                if (this.isProcessingQueue)
                {
                    return;
                }

                lock (this.gate)
                {
                    if (this.isProcessingQueue)
                    {
                        return;
                    }

                    this.isProcessingQueue = true;
                    DirtyTrackerNode node;
                    while (queue.TryDequeue(out node))
                    {
                        if (node.IsDirty)
                        {
                            Copy.PropertyValues(node.X, node.Y, this.Settings);
                        }
                    }

                    this.isProcessingQueue = false;
                }
            }

            private void OnTargetChanged(object sender, EventArgs e)
            {
                // think we want to track and throw here.
                // this is not perfect as some other change can trigger isProcessingQueue = true
                // keeping it simple
                if (this.isProcessingQueue)
                {
                    return;
                }

                var message = "Target cannot be modified when a synchronizer is applied to it\r\n" +
                              "The change would just trigger a dirty notification and the value would be updated with the value from source.";
                throw new InvalidOperationException(message);
            }
        }
    }
}