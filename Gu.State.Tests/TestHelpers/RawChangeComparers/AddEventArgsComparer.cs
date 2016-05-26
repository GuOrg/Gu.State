namespace Gu.State.Tests
{
    using NUnit.Framework;

    public class AddEventArgsComparer : EventArgsComparer<AddEventArgs>
    {
        public static readonly AddEventArgsComparer Default = new AddEventArgsComparer();

        public override bool Equals(AddEventArgs expected, AddEventArgs actual)
        {
            if (expected.Index != actual.Index)
            {
                throw new AssertionException($"Expected index to be {expected.Index} but was {actual.Index}");
            }

            return true;
        }
    }
}