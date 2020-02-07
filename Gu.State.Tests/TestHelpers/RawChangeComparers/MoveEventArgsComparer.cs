namespace Gu.State.Tests
{
    using NUnit.Framework;

    public sealed class MoveEventArgsComparer : EventArgsComparer<MoveEventArgs>
    {
        public static readonly MoveEventArgsComparer Default = new MoveEventArgsComparer();

        public override bool Equals(MoveEventArgs expected, MoveEventArgs actual)
        {
            if (!ReferenceEquals(expected.Source, actual.Source))
            {
                throw new AssertionException($"Expected source to be same.");
            }

            if (expected.FromIndex != actual.FromIndex)
            {
                throw new AssertionException($"Expected FromIndex to be {expected.FromIndex} but was {actual.FromIndex}");
            }

            if (expected.ToIndex != actual.ToIndex)
            {
                throw new AssertionException($"Expected ToIndex to be {expected.FromIndex} but was {actual.FromIndex}");
            }

            return true;
        }
    }
}