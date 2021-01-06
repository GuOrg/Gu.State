namespace Gu.State.Tests
{
    using NUnit.Framework;

    public sealed class ReplaceEventArgsComparer : EventArgsComparer<ReplaceEventArgs>
    {
        public static readonly ReplaceEventArgsComparer Default = new ReplaceEventArgsComparer();

        public override bool Equals(ReplaceEventArgs x, ReplaceEventArgs y)
        {
            if (!ReferenceEquals(x.Source, y.Source))
            {
                throw new AssertionException($"Expected source to be same.");
            }

            if (x.Index != y.Index)
            {
                throw new AssertionException($"Expected index to be {x.Index} but was {y.Index}");
            }

            return true;
        }
    }
}
