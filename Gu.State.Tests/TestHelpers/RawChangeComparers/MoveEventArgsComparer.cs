namespace Gu.State.Tests
{
    public class MoveEventArgsComparer : EventArgsComparer<MoveEventArgs>
    {
        public static readonly MoveEventArgsComparer Default = new MoveEventArgsComparer();

        public override bool Equals(MoveEventArgs x, MoveEventArgs y)
        {
            return x.FromIndex == y.FromIndex && x.ToIndex == y.ToIndex;
        }
    }
}