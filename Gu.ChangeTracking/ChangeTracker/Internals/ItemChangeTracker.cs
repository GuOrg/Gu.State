namespace Gu.ChangeTracking
{
    using System.ComponentModel;
    using System.Diagnostics;

    [DebuggerDisplay("ItemChangeTracker")]
    internal class ItemChangeTracker : ChangeTracker
    {
        public ItemChangeTracker(INotifyPropertyChanged source, int index, ChangeTracker parent)
            : base(source, parent.Settings, parent.Path.WithIndex(index))
        {
            this.Index = index;
            this.Parent = parent;
        }

        public int Index { get; }

        internal ChangeTracker Parent { get; }

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