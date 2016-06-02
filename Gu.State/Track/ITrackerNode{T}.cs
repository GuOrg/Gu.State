namespace Gu.State
{
    using System;

    internal interface ITrackerNode<T> : IDisposable
    {
        event EventHandler<TrackerChangedEventArgs<T>> Changed;
    }
}
