namespace Gu.State.Tests
{
    public class PropertyGraphChangedEventArgsComparer<TNode> : EventArgsComparer<PropertyGraphChangedEventArgs<TNode>>
    {
        public static readonly PropertyGraphChangedEventArgsComparer<TNode> Default = new PropertyGraphChangedEventArgsComparer<TNode>();

        private PropertyGraphChangedEventArgsComparer()
        {
        }

        public override bool Equals(PropertyGraphChangedEventArgs<TNode> x, PropertyGraphChangedEventArgs<TNode> y)
        {
            if (!ReferenceEquals(x.Node, y.Node))
            {
                return false;
            }

            if (x.Previous != null && !EventArgsComparer.Default.Equals(x.Previous, y.Previous))
            {
                return false;
            }

            return EventArgsComparer.Default.Equals(x.Property, y.Property);
        }
    }
}