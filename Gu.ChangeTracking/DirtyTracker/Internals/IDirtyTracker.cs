namespace Gu.ChangeTracking
{
    internal interface IDirtyTracker : IDirtyTrackerNode
    {
        PropertiesSettings Settings { get; }

        void Update(IDirtyTrackerNode child);
    }
}