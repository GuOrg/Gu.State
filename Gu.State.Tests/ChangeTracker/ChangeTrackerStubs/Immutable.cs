namespace Gu.State.Tests.ChangeTrackerStubs
{
    public sealed class Immutable
    {
        public readonly int Value;

        public Immutable()
        {
        }

        public Immutable(int value)
        {
            this.Value = value;
        }
    }
}