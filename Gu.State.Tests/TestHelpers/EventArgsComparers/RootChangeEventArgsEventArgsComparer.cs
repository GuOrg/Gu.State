namespace Gu.State.Tests
{
    using NUnit.Framework;

    public class RootChangeEventArgsEventArgsComparer<TNode> : EventArgsComparer<RootChangeEventArgs<TNode>>
    {
        public static readonly RootChangeEventArgsEventArgsComparer<TNode> Default = new RootChangeEventArgsEventArgsComparer<TNode>();

        public override bool Equals(RootChangeEventArgs<TNode> expected, RootChangeEventArgs<TNode> actual)
        {
            if (!ReferenceEquals(expected.Node, actual.Node))
            {
                throw new AssertionException($"Expected actual.Node to be same as expected.Node");
            }

            if (!EventArgsComparer.Default.Equals(expected.Previous, actual.Previous))
            {
                return false;
            }

            return EventArgsComparer.Default.Equals(expected.EventArgs, actual.EventArgs);
        }
    }
}