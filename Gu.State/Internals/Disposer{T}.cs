namespace Gu.State
{
    using System;
    using System.Diagnostics;

    internal sealed class Disposer<T> : IDisposer<T>, IBorrowed<T>
    {
        private readonly Action<T> dispose;
        private readonly object gate = new object();
        private readonly T value;
        private bool disposed;

        internal Disposer(T value, Action<T> dispose)
        {
            Debug.Assert(dispose != null, "dispose == null");
            this.value = value;
            this.dispose = dispose;
        }

        public T Value
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException($"Not allowed to get the value of a {this.GetType().PrettyName()} after it is disposed.");
                }

                return this.value;
            }
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
                this.dispose(this.value);
            }
        }
    }
}
