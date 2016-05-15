namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;

    public abstract class EventArgsComparer<T> : IEqualityComparer<T>
    {
        public Type Type => typeof(T);

        public abstract bool Equals(T x, T y);

        public int GetHashCode(T obj)
        {
            throw new NotImplementedException();
        }
    }
}