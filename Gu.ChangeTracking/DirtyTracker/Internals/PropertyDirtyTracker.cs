namespace Gu.ChangeTracking
{
    using System.ComponentModel;
    using System.Reflection;

    internal class PropertyDirtyTracker : DirtyTracker<INotifyPropertyChanged>, IDirtyTrackerNode
    {
        private readonly IDirtyTrackerNode parent;

        public PropertyDirtyTracker(INotifyPropertyChanged x, INotifyPropertyChanged y, IDirtyTrackerNode parent, PropertyInfo propertyInfo)
            : base(x, y, parent.Settings, false)
        {
            this.parent = parent;
            this.PropertyInfo = propertyInfo;
        }

        public PropertyInfo PropertyInfo { get; }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            if (propertyName == nameof(this.IsDirty))
            {
                this.parent.Update(this);
            }

            base.OnPropertyChanged(propertyName);
        }
    }
}