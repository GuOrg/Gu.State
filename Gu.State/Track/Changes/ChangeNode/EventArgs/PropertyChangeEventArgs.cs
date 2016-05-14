namespace Gu.State
{
    using System.Reflection;

    public struct PropertyChangeEventArgs : IRootChangeEventArgs
    {
        public PropertyChangeEventArgs(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
        }

        public PropertyInfo PropertyInfo { get; }
    }
}