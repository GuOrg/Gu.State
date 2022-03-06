namespace Gu.State.Tests
{
    using NUnit.Framework;

    public sealed class PropertyGraphChangedEventArgsComparer<TNode> : EventArgsComparer<PropertyGraphChangedEventArgs<TNode>>
    {
        public static readonly PropertyGraphChangedEventArgsComparer<TNode> Default = new();

        private PropertyGraphChangedEventArgsComparer()
        {
        }

        public override bool Equals(PropertyGraphChangedEventArgs<TNode> x, PropertyGraphChangedEventArgs<TNode> y)
        {
            if (!ReferenceEquals(x.Node, y.Node))
            {
                throw new AssertionException($"Expected x.Node to be same as y.Node");
            }

            if (!EventArgsComparer.Default.Equals(x.Previous, y.Previous))
            {
                return false;
            }

            if (x.Property != y.Property)
            {
                throw new AssertionException($"Expected property {x.Property.Name} but was {y.Property.Name}");
            }

            return true;
        }
    }
}
