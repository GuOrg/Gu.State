namespace Gu.State.Tests
{
    using NUnit.Framework;

    public sealed class RootChangeEventArgsEventArgsComparer<TNode> : EventArgsComparer<RootChangeEventArgs<TNode>>
    {
        public static readonly RootChangeEventArgsEventArgsComparer<TNode> Default = new();

        public override bool Equals(RootChangeEventArgs<TNode> x, RootChangeEventArgs<TNode> y)
        {
            if (!ReferenceEquals(x.Node, y.Node))
            {
                throw new AssertionException($"Expected x.Node to be same as y.Node");
            }

            if (!EventArgsComparer.Default.Equals(x.Previous, y.Previous))
            {
                return false;
            }

            return EventArgsComparer.Default.Equals(x.EventArgs, y.EventArgs);
        }
    }
}
