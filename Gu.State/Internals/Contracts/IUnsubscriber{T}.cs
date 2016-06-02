namespace Gu.State
{
    internal interface IUnsubscriber<out T> : IDisposer<T>, IUnsubscriber
    {
    }
}