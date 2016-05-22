namespace Gu.State.Tests
{
    using NUnit.Framework;

    public class PropertyGraphChangedEventArgsComparer<TNode> : EventArgsComparer<PropertyGraphChangedEventArgs<TNode>>
    {
        public static readonly PropertyGraphChangedEventArgsComparer<TNode> Default = new PropertyGraphChangedEventArgsComparer<TNode>();

        private PropertyGraphChangedEventArgsComparer()
        {
        }

        public override bool Equals(PropertyGraphChangedEventArgs<TNode> expected, PropertyGraphChangedEventArgs<TNode> actual)
        {
            if (!ReferenceEquals(expected.Node, actual.Node))
            {
                throw new AssertionException($"Expected actual.Node to be same as expected.Node");
            }

            if (!EventArgsComparer.Default.Equals(expected.Previous, actual.Previous))
            {
                return false;
            }

            return EventArgsComparer.Default.Equals(expected.Property, actual.Property);
        }
    }
}