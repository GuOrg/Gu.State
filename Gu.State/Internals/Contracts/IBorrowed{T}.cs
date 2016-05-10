namespace Gu.State
{
    internal interface IBorrowed<out T> : IDisposer<T>
    {
    }
}