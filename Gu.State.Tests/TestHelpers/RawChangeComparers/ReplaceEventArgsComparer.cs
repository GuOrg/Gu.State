namespace Gu.State.Tests
{
    public class ReplaceEventArgsComparer : EventArgsComparer<ReplaceEventArgs>
    {
        public static readonly ReplaceEventArgsComparer Default = new ReplaceEventArgsComparer();

        public override bool Equals(ReplaceEventArgs x, ReplaceEventArgs y)
        {
            return x.Index == y.Index;
        }
    }
}