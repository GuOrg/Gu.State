namespace Gu.State
{
    using System;

    internal static class Unsubscriber
    {
        internal static IUnsubscriber<T> AsUnsubscribeOnDispose<T>(this T source, Action<T> unsubscribe)
            where T : IDisposable
        {
            return new Disposer<T>(source, unsubscribe);
        }
    }
}