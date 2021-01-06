#pragma warning disable SA1600 // Elements should be documented
namespace Gu.State
{
    using System;

    internal interface IChildNode<T> : IDisposable
        where T : ITrackerNode<T>
    {
        event EventHandler<TrackerChangedEventArgs<T>> Changed;

        T TrackerNode { get; }
    }
}
