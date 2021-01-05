namespace Gu.State.Tests
{
    using NUnit.Framework;

    public sealed class RemoveEventArgsComparer : EventArgsComparer<RemoveEventArgs>
    {
        public static readonly RemoveEventArgsComparer Default = new RemoveEventArgsComparer();

        public override bool Equals(RemoveEventArgs x, RemoveEventArgs y)
        {
            if (!ReferenceEquals(x.Source, y.Source))
            {
                throw new AssertionException("Expected source to be same.");
            }

            if (x.Index != y.Index)
            {
                throw new AssertionException($"Expected index to be {x.Index} but was {y.Index}");
            }

            return true;
        }
    }
}
