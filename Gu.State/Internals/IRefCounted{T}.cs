namespace Gu.State
{
    using System;

    internal interface IRefCounted<out T> : IDisposable
    {
        T Value { get; }

        int Count { get; }
    }
}