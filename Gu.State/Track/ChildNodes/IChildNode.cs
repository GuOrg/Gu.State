namespace Gu.State
{
    using System;

    internal interface IChildNode : IDisposable
    {
        event EventHandler<TrackerChangedEventArgs<ChangeTrackerNode>> Changed;
    }
}