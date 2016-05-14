namespace Gu.State
{
    using System;
    using System.Diagnostics;

    internal sealed class Disposer : IDisposable
    {
        private readonly Action dispose;
        private readonly object gate = new object();
        private bool disposed;

        public Disposer(Action dispose)
        {
            Debug.Assert(dispose != null, "dispose == null");
            this.dispose = dispose;
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            lock (this.gate)
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;
                this.dispose();
            }
        }
    }
}
