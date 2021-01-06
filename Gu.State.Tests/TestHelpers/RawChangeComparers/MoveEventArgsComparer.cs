namespace Gu.State.Tests
{
    using NUnit.Framework;

    public sealed class MoveEventArgsComparer : EventArgsComparer<MoveEventArgs>
    {
        public static readonly MoveEventArgsComparer Default = new MoveEventArgsComparer();

        public override bool Equals(MoveEventArgs x, MoveEventArgs y)
        {
            if (!ReferenceEquals(x.Source, y.Source))
            {
                throw new AssertionException($"Expected source to be same.");
            }

            if (x.FromIndex != y.FromIndex)
            {
                throw new AssertionException($"Expected FromIndex to be {x.FromIndex} but was {y.FromIndex}");
            }

            if (x.ToIndex != y.ToIndex)
            {
                throw new AssertionException($"Expected ToIndex to be {x.FromIndex} but was {y.FromIndex}");
            }

            return true;
        }
    }
}
