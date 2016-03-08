namespace Gu.State
{
    using System;

    internal interface IRootNode<TKey, TTracker> : IDisposable
        where TTracker : ITracker
    {
        RefCountCollection<TKey, TTracker> Cache { get; }
    }
}