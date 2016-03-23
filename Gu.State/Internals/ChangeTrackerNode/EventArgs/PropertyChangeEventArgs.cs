namespace Gu.State
{
    using System.Reflection;

    internal struct PropertyChangeEventArgs
    {
        internal PropertyInfo PropertyInfo;

        public PropertyChangeEventArgs(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
        }
    }
}