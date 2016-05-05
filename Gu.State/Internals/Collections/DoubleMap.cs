namespace Gu.State
{
    using System.Collections.Generic;

    internal class DoubleMap<T1, T2>
    {
        private readonly Dictionary<T1, T2> t1t2 = new Dictionary<T1, T2>();
        private readonly Dictionary<T2, T1> t2t1 = new Dictionary<T2, T1>();
        private readonly object gate = new object();

        internal void Add(T1 t1, T2 t2)
        {
            lock (this.gate)
            {
                this.t1t2.Add(t1, t2);
                this.t2t1.Add(t2, t1);
            }
        }

        internal void Add(T2 t2, T1 t1)
        {
            this.Add(t1, t2);
        }

        internal bool TryRemove(T1 t1)
        {
            lock (this.gate)
            {
                return this.TryRemove(t1, this.t1t2[t1]);
            }
        }

        internal bool TryRemove(T2 t2)
        {
            lock (this.gate)
            {
                return this.TryRemove(this.t2t1[t2], t2);
            }
        }

        private bool TryRemove(T1 t1, T2 t2)
        {
            return this.t1t2.Remove(t1) && this.t2t1.Remove(t2);
        }
    }
}
