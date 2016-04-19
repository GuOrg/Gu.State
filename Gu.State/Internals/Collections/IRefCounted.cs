namespace Gu.State
{
    internal interface IRefCounted<out TValue>
        where TValue : IRefCountable
    {
        TValue Tracker { get; }

        void RemoveOwner<TOwner>(TOwner owner)
            where TOwner : class;
    }
}