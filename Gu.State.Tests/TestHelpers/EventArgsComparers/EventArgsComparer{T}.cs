namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;

    public abstract class EventArgsComparer<T> : IEqualityComparer<T>
    {
        public Type Type => typeof(T);

        public abstract bool Equals(T expected, T actual);

        public int GetHashCode(T obj)
        {
            throw new NotImplementedException();
        }
    }
}