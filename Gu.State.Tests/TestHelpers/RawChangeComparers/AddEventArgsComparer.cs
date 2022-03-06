namespace Gu.State.Tests
{
    using NUnit.Framework;

    public sealed class AddEventArgsComparer : EventArgsComparer<AddEventArgs>
    {
        public static readonly AddEventArgsComparer Default = new();

        public override bool Equals(AddEventArgs x, AddEventArgs y)
        {
            if (!ReferenceEquals(x.Source, y.Source))
            {
                throw new AssertionException($"Expected source to be same.");
            }

            if (x.Index != y.Index)
            {
                throw new AssertionException($"Expected index to be {x.Index} but was {y.Index}");
            }

            return true;
        }
    }
}
