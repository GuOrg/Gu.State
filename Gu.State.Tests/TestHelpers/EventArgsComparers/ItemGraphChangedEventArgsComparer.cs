namespace Gu.State.Tests
{
    using NUnit.Framework;

    public sealed class ItemGraphChangedEventArgsComparer<TNode> : EventArgsComparer<ItemGraphChangedEventArgs<TNode>>
    {
        public static readonly ItemGraphChangedEventArgsComparer<TNode> Default = new ItemGraphChangedEventArgsComparer<TNode>();

        private ItemGraphChangedEventArgsComparer()
        {
        }

        public override bool Equals(ItemGraphChangedEventArgs<TNode> x, ItemGraphChangedEventArgs<TNode> y)
        {
            if (!ReferenceEquals(x.Node, y.Node))
            {
                throw new AssertionException($"Expected x.Node to be same as y.Node");
            }

            if (!EventArgsComparer.Default.Equals(x.Previous, y.Previous))
            {
                return false;
            }

            if (x.Index != y.Index)
            {
                throw new AssertionException($"Expected index to be {x.Index} but was {y.Index}");
            }

            return true;
        }
    }
}
