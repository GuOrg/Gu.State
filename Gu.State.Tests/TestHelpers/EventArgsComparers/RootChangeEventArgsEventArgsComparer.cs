namespace Gu.State.Tests
{
    public class RootChangeEventArgsEventArgsComparer<TNode> : EventArgsComparer<RootChangeEventArgs<TNode>>
    {
        public static readonly RootChangeEventArgsEventArgsComparer<TNode> Default = new RootChangeEventArgsEventArgsComparer<TNode>();

        public override bool Equals(RootChangeEventArgs<TNode> expected, RootChangeEventArgs<TNode> actual)
        {
            if (!ReferenceEquals(expected.Node, actual.Node))
            {
                return false;
            }

            if (expected.Previous != null && !EventArgsComparer.Default.Equals(expected.Previous, actual.Previous))
            {
                return false;
            }

            return EventArgsComparer.Default.Equals(expected.EventArgs, actual.EventArgs);
        }
    }
}