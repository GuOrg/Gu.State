namespace Gu.State
{
    using System.Reflection;

    internal struct UpdateEventArgs
    {
        public UpdateEventArgs(PropertyInfo propertyInfo, object item)
        {
            this.PropertyInfo = propertyInfo;
            this.Item = item;
        }

        public PropertyInfo PropertyInfo { get; }

        public object Item { get; }
    }
}