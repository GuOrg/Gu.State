namespace Gu.State
{
    using System;

    internal interface IUnsubscriber<out T> : IDisposable
    {
        T Value { get; }
    }
}