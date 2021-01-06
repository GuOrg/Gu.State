#pragma warning disable SA1600 // Elements should be documented
namespace Gu.State
{
    using System;

    internal interface ITrackerNode<T> : IDisposable
    {
        event EventHandler<TrackerChangedEventArgs<T>> Changed;
    }
}
