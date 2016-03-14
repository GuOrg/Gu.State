namespace Gu.State
{
    using System;

    internal interface IChangeTrackerNode
    {
        event EventHandler<UpdateEventArgs> ChildUpdate;

        event EventHandler<ResetEventArgs> ChildReset;

        event EventHandler<AddEventArgs> ChildAdd;

        event EventHandler<RemoveEventArgs> ChildRemove;

        event EventHandler<MoveEventArgs> ChildMove;

        event EventHandler<EventArgs> Change;
    }
}