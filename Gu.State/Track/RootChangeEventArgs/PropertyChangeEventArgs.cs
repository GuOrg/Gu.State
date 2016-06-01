namespace Gu.State
{
    using System.Reflection;

    public struct PropertyChangeEventArgs : IRootChangeEventArgs
    {
        public PropertyChangeEventArgs(object source, PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
            this.Source = source;
        }

        /// <inheritdoc />
        public object Source { get; }

        public PropertyInfo PropertyInfo { get; }
    }
}