namespace Gu.State.Tests
{
    using NUnit.Framework;

    public class ItemGraphChangedEventArgsComparer<TNode> : EventArgsComparer<ItemGraphChangedEventArgs<TNode>>
    {
        public static readonly ItemGraphChangedEventArgsComparer<TNode> Default = new ItemGraphChangedEventArgsComparer<TNode>();

        private ItemGraphChangedEventArgsComparer()
        {
        }

        public override bool Equals(ItemGraphChangedEventArgs<TNode> expected, ItemGraphChangedEventArgs<TNode> actual)
        {
            if (!ReferenceEquals(expected.Node, actual.Node))
            {
                throw new AssertionException($"Expected actual.Node to be same as expected.Node");
            }

            if (!EventArgsComparer.Default.Equals(expected.Previous, actual.Previous))
            {
                return false;
            }

            if (expected.Index != actual.Index)
            {
                throw new AssertionException($"Expected index to be {expected.Index} but was {actual.Index}");
            }

            return true;
        }
    }
}