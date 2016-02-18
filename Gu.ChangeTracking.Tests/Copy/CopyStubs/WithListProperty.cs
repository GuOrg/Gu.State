namespace Gu.ChangeTracking.Tests.CopyStubs
{
    using System.Collections.Generic;

    public class WithListProperty<T>
    {
        public List<T> Items { get; } = new List<T>();
    }
}
