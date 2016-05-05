namespace Gu.State
{
    using System;
    using System.Diagnostics;

    internal sealed class Disposer<T> : IDisposable
    {
        internal readonly T Value;
        private readonly Action<T> dispose;
        private readonly object gate = new object();
        private bool disposed;

        internal Disposer(T value, Action<T> dispose)
        {
            Debug.Assert(dispose != null, "dispose == null");
            this.Value = value;
            this.dispose = dispose;
        }

        public void Dispose()
        {
            lock (this.gate)
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;
                this.dispose(this.Value);
            }
        }
    }
}
