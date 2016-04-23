namespace Gu.State
{
    using System;
    using System.Diagnostics;

    internal struct Disposer<T> : IDisposable
    {
        internal readonly T Value;
        private readonly Action<T> action;

        internal Disposer(T value, Action<T> action)
        {
            Debug.Assert(action != null, "action == null");
            this.Value = value;
            this.action = action;
        }

        public void Dispose()
        {
            this.action(this.Value);
        }
    }
}
