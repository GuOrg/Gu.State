namespace Gu.State
{
    using System;

    internal interface INode<TTracker> : IDisposable
        where TTracker : ITracker
    {
        IRootNode<TTracker> Root { get; }

        TTracker Tracker { get; }

        INode<TTracker> AddChild(object source, Func<TTracker> trackerFactory);

        void RemoveChild(object key);
    }
}