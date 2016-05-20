namespace Gu.State.Tests
{
    public class RemoveEventArgsComparer : EventArgsComparer<RemoveEventArgs>
    {
        public static readonly RemoveEventArgsComparer Default = new RemoveEventArgsComparer();

        public override bool Equals(RemoveEventArgs x, RemoveEventArgs y)
        {
            return x.Index == y.Index;
        }
    }
}