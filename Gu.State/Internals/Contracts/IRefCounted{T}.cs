#pragma warning disable SA1600 // Elements should be documented
namespace Gu.State
{
    using System;

    internal interface IRefCounted<out T> : IDisposable
    {
        T Value { get; }

        int Count { get; }
    }
}
