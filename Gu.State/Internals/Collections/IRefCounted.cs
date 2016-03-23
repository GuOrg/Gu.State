namespace Gu.State
{
    interface IRefCounted<TValue>
        where TValue : IRefCountable
    {
        TValue Tracker { get; }

        void RemoveOwner<TOwner>(TOwner owner)
            where TOwner : class;
    }
}