namespace Gu.State.Tests
{
    public class AddEventArgsComparer : EventArgsComparer<AddEventArgs>
    {
        public static readonly AddEventArgsComparer Default = new AddEventArgsComparer();

        public override bool Equals(AddEventArgs x, AddEventArgs y)
        {
            return x.Index == y.Index;
        }
    }
}