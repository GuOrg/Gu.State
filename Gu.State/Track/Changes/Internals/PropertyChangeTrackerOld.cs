namespace Gu.State
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("PropertyTracker Property: {PropertyInfo.Name}")]
    internal class PropertyChangeTrackerOld : ChangeTracker
    {
        public PropertyChangeTrackerOld(INotifyPropertyChanged source, PropertyInfo propertyInfo, ChangeTracker parent)
            : base(source, parent.Settings, parent.Path.WithProperty(propertyInfo))
        {
            this.Parent = parent;
            this.PropertyInfo = propertyInfo;
        }

        internal ChangeTracker Parent { get; }

        public PropertyInfo PropertyInfo { get; }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.Changes))
            {
                this.Parent.Changes++;
            }

            base.OnPropertyChanged(e);
        }
    }
}