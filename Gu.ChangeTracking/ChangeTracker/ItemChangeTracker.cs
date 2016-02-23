namespace Gu.ChangeTracking
{
    using System.ComponentModel;
    using System.Diagnostics;

    [DebuggerDisplay("ItemChangeTracker")]
    internal class ItemChangeTracker : ChangeTracker
    {
        private readonly ChangeTracker parent;

        public ItemChangeTracker(INotifyPropertyChanged source, ChangeTracker parent)
            : base(source, parent.Settings)
        {
            this.parent = parent;
        }

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