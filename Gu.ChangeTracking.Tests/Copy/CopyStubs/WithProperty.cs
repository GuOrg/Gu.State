namespace Gu.ChangeTracking.Tests.CopyStubs
{
    public class WithProperty<T>
    {
        public WithProperty()
        {
        }

        public WithProperty(T value)
        {
            this.Value = value;
        }

        public T Value { get; set; }
    }
}