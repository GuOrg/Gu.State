namespace Gu.State.Tests
{
    using NUnit.Framework;

    public sealed class PropertyChangedEventArgsComparer : EventArgsComparer<PropertyChangeEventArgs>
    {
        public static readonly PropertyChangedEventArgsComparer Default = new PropertyChangedEventArgsComparer();

        public override bool Equals(PropertyChangeEventArgs x, PropertyChangeEventArgs y)
        {
            if (!ReferenceEquals(x.Source, y.Source))
            {
                throw new AssertionException($"Expected source to be same.");
            }

            if (x.PropertyInfo != y.PropertyInfo)
            {
                throw new AssertionException($"Expected property {x.PropertyInfo.Name} but was {y.PropertyInfo.Name}");
            }

            return true;
        }
    }
}
