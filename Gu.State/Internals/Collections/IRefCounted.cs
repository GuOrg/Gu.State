namespace Gu.State
{
    using System;
    interface IRefCounted<TValue> : IDisposable
    {
        TValue Tracker { get; }

        CanDispose RemoveOwner<TOwner>(TOwner owner)
            where TOwner : class;
    }
}