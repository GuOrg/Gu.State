namespace Gu.ChangeTracking.Tests.DirtyTrackerStubs
{
    public struct WithGetReadOnlyPropertyStruct<T>
    {
        public WithGetReadOnlyPropertyStruct(T value)
        {
            this.Value = value;
        }

        public T Value { get; }
    }
}