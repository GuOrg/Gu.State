namespace Gu.ChangeTracking
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("PropertyDirtyTracker Property: {PropertyInfo.Name}, IsDirty: {IsDirty}")]
    internal class PropertyDirtyTracker : DirtyTracker<INotifyPropertyChanged>, IDirtyTracker
    {
        private readonly IDirtyTracker parent;

        public PropertyDirtyTracker(INotifyPropertyChanged x, INotifyPropertyChanged y, IDirtyTracker parent, PropertyInfo propertyInfo)
            : base(x, y, parent.Settings)
        {
            this.parent = parent;
            this.PropertyInfo = propertyInfo;
        }

        public PropertyInfo PropertyInfo { get; }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            if (propertyName == nameof(this.IsDirty))
            {
                this.parent?.Update(this);
            }

            base.OnPropertyChanged(propertyName);
        }
    }
}