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
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
            throw new NotSupportedException();
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
        }
    }
}