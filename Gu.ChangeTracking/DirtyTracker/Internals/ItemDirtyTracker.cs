namespace Gu.ChangeTracking
{
    using System.Collections;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    internal class ItemDirtyTracker : DirtyTracker<INotifyPropertyChanged>, IDirtyTrackerNode
    {
        internal static readonly PropertyInfo IndexerProperty = typeof(IList).GetProperties().Single(p => p.GetIndexParameters().Length > 0);
        private readonly IDirtyTrackerNode parent;

        public ItemDirtyTracker(INotifyPropertyChanged x, INotifyPropertyChanged y, IDirtyTrackerNode parent)
            : base(x, y, parent.Settings, false)
        {
            this.parent = parent;
        }

        public PropertyInfo PropertyInfo => IndexerProperty;

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