namespace Gu.State
{
    using System;

    internal struct Disposer<T> : IDisposable
    {
        internal readonly T Value;
        private readonly Action<T> action;

        public Disposer(T value, Action<T> action)
        {
            this.Value = value;
            this.action = action;
        }

        public void Dispose()
        {
            this.action(this.Value);
        }
    }
}
