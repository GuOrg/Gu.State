namespace Gu.State.Tests
{
    using NUnit.Framework;

    public sealed class ReplaceEventArgsComparer : EventArgsComparer<ReplaceEventArgs>
    {
        public static readonly ReplaceEventArgsComparer Default = new ReplaceEventArgsComparer();

        public override bool Equals(ReplaceEventArgs expected, ReplaceEventArgs actual)
        {
            if (!ReferenceEquals(expected.Source, actual.Source))
            {
                throw new AssertionException($"Expected source to be same.");
            }

            if (expected.Index != actual.Index)
            {
                throw new AssertionException($"Expected index to be {expected.Index} but was {actual.Index}");
            }

            return true;
        }
    }
}