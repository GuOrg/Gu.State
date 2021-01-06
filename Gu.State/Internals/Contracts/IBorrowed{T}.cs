#pragma warning disable SA1600 // Elements should be documented
namespace Gu.State
{
    internal interface IBorrowed<out T> : IDisposer<T>
    {
    }
}
