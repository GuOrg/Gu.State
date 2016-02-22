namespace Gu.ChangeTracking.Tests.DirtyTrackerStubs
{
    public sealed class WithGetReadOnlyPropertySealed<T>
    {
        public WithGetReadOnlyPropertySealed(T value)
        {
            this.Value = value;
        }

        public T Value { get; }
    }
}