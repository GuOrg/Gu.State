namespace Gu.State
{
    using System.Collections.Generic;

    internal class DoubleMap<T1, T2>
    {
        private readonly Dictionary<T1, T2> t1t2 = new Dictionary<T1, T2>();
        private readonly Dictionary<T2, T1> t2t1 = new Dictionary<T2, T1>();
    }
}
