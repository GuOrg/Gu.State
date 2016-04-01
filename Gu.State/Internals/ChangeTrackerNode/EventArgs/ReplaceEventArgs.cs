namespace Gu.State
{
    internal struct ReplaceEventArgs
    {
        public ReplaceEventArgs(int index, object oldItem, object newItem)
        {
            this.Index = index;
            this.OldItem = oldItem;
            this.NewItem = newItem;
        }

        public int Index { get; }

        public object OldItem { get;  }

        public object NewItem { get;  }
    }
}