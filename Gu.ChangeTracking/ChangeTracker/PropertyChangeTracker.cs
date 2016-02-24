namespace Gu.ChangeTracking
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("PropertyTracker Property: {PropertyInfo.Name}")]
    internal class PropertyChangeTracker : ChangeTracker
    {
        public PropertyChangeTracker(INotifyPropertyChanged source, PropertyInfo propertyInfo, ChangeTracker parent)
            : base(source, parent.Settings, parent.Path.WithProperty(propertyInfo))
        {
            this.Parent = parent;
            this.PropertyInfo = propertyInfo;
        }

        internal ChangeTracker Parent { get; }

        public PropertyInfo PropertyInfo { get; }

        public override int Changes
        {
            get
            {
                return base.Changes;
            }
            protected internal set
            {
                this.Parent.Changes++;
                base.Changes = value;
            }
        }
    }
}