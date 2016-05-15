namespace Gu.State.Tests
{
    public class RootChangeEventArgsEventArgsComparer<TNode> : EventArgsComparer<RootChangeEventArgs<TNode>>
    {
        public static readonly RootChangeEventArgsEventArgsComparer<TNode> Default = new RootChangeEventArgsEventArgsComparer<TNode>();

        public override bool Equals(RootChangeEventArgs<TNode> x, RootChangeEventArgs<TNode> y)
        {
            if (!ReferenceEquals(x.Node, y.Node))
            {
                return false;
            }

            if (x.Previous != null && !EventArgsComparer.Default.Equals(x.Previous, y.Previous))
            {
                return false;
            }

            return EventArgsComparer.Default.Equals(x.EventArgs, y.EventArgs);
        }
    }
}