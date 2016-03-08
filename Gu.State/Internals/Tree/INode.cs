namespace Gu.State
{
    using System;

    internal interface INode<TKey, TTracker> : IDisposable
        where TTracker : ITracker
    {
        IRootNode<TKey, TTracker> Root { get; }

        TTracker Tracker { get; }

        void AddChild(TKey key, Func<TTracker> trackerFactory);

        void RemoveChild(TKey key);
    }
}