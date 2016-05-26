namespace Gu.State.Tests
{
    using NUnit.Framework;

    public class ReplaceEventArgsComparer : EventArgsComparer<ReplaceEventArgs>
    {
        public static readonly ReplaceEventArgsComparer Default = new ReplaceEventArgsComparer();

        public override bool Equals(ReplaceEventArgs expected, ReplaceEventArgs actual)
        {
            if (expected.Index != actual.Index)
            {
                throw new AssertionException($"Expected index to be {expected.Index} but was {actual.Index}");
            }

            return true;
        }
    }
}