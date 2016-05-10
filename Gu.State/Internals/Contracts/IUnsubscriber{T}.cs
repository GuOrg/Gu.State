namespace Gu.State
{
    internal interface IUnsubscriber<out T> : IUnsubscriber
    {
        T Value { get; }
    }
}