namespace Gu.State
{
    using System;

    internal sealed class Disposer : IDisposable
    {
        private readonly Action action;

        public Disposer(Action action)
        {
            this.action = action;
        }

        public static Disposer<T> Create<T>(T value, Action<T> onDispose)
        {
            return new Disposer<T>(value, onDispose);
        }

        public void Dispose()
        {
            this.action();
        }
    }
}
