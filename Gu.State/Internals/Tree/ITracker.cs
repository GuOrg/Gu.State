namespace Gu.State
{
    using System;

    interface ITracker : IDisposable
    {
        event EventHandler Changed;

        void ChildChanged(ITracker child);
    }
}