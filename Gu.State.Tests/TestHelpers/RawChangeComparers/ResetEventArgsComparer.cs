namespace Gu.State.Tests
{
    using NUnit.Framework;

    public sealed class ResetEventArgsComparer : EventArgsComparer<ResetEventArgs>
    {
        public static readonly ResetEventArgsComparer Default = new ResetEventArgsComparer();

        public override bool Equals(ResetEventArgs x, ResetEventArgs y)
        {
            if (!ReferenceEquals(x.Source, y.Source))
            {
                throw new AssertionException($"Expected source to be same.");
            }

            return true;
        }
    }
}
