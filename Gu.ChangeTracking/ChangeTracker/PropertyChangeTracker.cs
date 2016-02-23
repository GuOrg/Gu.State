namespace Gu.ChangeTracking
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("PropertyTracker Property: {PropertyInfo.Name}")]
    internal class PropertyChangeTracker : ChangeTracker
    {
        private readonly ChangeTracker parent;

        public PropertyChangeTracker(INotifyPropertyChanged source, PropertyInfo propertyInfo, ChangeTracker parent)
            : base(source, parent.Settings)
        {
            this.parent = parent;
            this.PropertyInfo = propertyInfo;
        }

        public PropertyInfo PropertyInfo { get; }

        public override int Changes
        {
            get
            {
                return base.Changes;
            }
            protected internal set
            {
                this.parent.Changes++;
                base.Changes = value;
            }
        }
    }
}