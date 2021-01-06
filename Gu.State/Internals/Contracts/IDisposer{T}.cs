#pragma warning disable SA1600 // Elements should be documented
namespace Gu.State
{
    using System;

    internal interface IDisposer<out T> : IDisposable
    {
        T Value { get; }
    }
}
