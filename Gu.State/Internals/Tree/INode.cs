namespace Gu.State
{
    using System;

    internal interface INode<TKey, TTracker> : IDisposable
        where TTracker : ITracker
    {
        IRootNode<TKey, TTracker> Root { get; }

        TTracker Tracker { get; }

        INode<TKey, TTracker> AddChild(TKey childKey, Func<TTracker> trackerFactory);

        void RemoveChild(TKey key);
    }
}