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

        public void Dispose()
        {
            this.action();
        }
    }
}
