namespace Gu.State
{
    internal interface IRefCounted<out T> : IDisposer<T>
    {
    }
}