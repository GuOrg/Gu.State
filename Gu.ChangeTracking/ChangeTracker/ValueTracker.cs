namespace Gu.ChangeTracking
{
    internal abstract class ValueTracker : ChangeTracker, IValueTracker
    {
        protected ValueTracker(object value)
        {
            Ensure.NotNull(value, nameof(value));
            this.Value = value;
        }

        public object Value { get; }
    }
}