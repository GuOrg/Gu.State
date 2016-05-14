namespace Gu.State
{
    using System.Reflection;

    internal struct PropertyChangeEventArgs : IRootChangeEventArgs
    {
        internal readonly PropertyInfo PropertyInfo;

        public PropertyChangeEventArgs(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
        }
    }
}