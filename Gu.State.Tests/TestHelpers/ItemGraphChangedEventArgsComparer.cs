namespace Gu.State.Tests
{
    public class ItemGraphChangedEventArgsComparer<TNode> : EventArgsComparer<ItemGraphChangedEventArgs<TNode>>
    {
        public static readonly ItemGraphChangedEventArgsComparer<TNode> Default = new ItemGraphChangedEventArgsComparer<TNode>();

        private ItemGraphChangedEventArgsComparer()
        {
        }

        public override bool Equals(ItemGraphChangedEventArgs<TNode> x, ItemGraphChangedEventArgs<TNode> y)
        {
            if (!ReferenceEquals(x.Node, y.Node))
            {
                return false;
            }

            if (x.Previous != null && !EventArgsComparer.Default.Equals(x.Previous, y.Previous))
            {
                return false;
            }

            return EventArgsComparer.Default.Equals(x.Index, y.Index);
        }
    }
}