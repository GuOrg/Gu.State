namespace Gu.State
{
    using System;

    internal interface IRootNode<TTracker> : IDisposable
        where TTracker : ITracker
    {
        RefCountCollection<IReference, TTracker> Cache { get; }
    }
}