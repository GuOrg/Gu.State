namespace Gu.ChangeTracking
{
    internal interface IDirtyTracker : IDirtyTrackerNode
    {
        DirtyTrackerSettings Settings { get; }

        void Update(IDirtyTrackerNode child);
    }
}