namespace Gu.State
{
    using System;

    interface IChildNode : IDisposable
    {
        event EventHandler<TrackerChangedEventArgs<ChangeTrackerNode>> Changed;
    }
}