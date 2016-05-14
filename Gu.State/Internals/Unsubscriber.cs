namespace Gu.State
{
    using System;

    internal static class Unsubscriber
    {
        internal static IUnsubscriber<T> UnsubscribeAndDispose<T>(this T source, Action<T> unsubscribe)
            where T : IDisposable
        {
            return new Disposer<T>(
                source,
                x =>
                    {
                        unsubscribe(x);
                        x.Dispose();
                    });
        }
    }
}