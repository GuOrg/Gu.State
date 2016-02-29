namespace Gu.ChangeTracking.Tests.CopyStubs
{
    public class WithReadonlyProperty<T>
    {
        public WithReadonlyProperty(T value)
        {
            this.Value = value;
        }

        public T Value { get; }
    }
}