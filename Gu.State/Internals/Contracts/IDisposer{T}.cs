namespace Gu.State
{
    using System;

    internal interface IDisposer<out T> : IDisposable
    {
        T Value { get; }
    }
}
