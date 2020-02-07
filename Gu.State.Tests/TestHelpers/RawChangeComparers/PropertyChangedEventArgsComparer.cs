namespace Gu.State.Tests
{
    using NUnit.Framework;

    public sealed class PropertyChangedEventArgsComparer : EventArgsComparer<PropertyChangeEventArgs>
    {
        public static readonly PropertyChangedEventArgsComparer Default = new PropertyChangedEventArgsComparer();

        public override bool Equals(PropertyChangeEventArgs expected, PropertyChangeEventArgs actual)
        {
            if (!ReferenceEquals(expected.Source, actual.Source))
            {
                throw new AssertionException($"Expected source to be same.");
            }

            if (expected.PropertyInfo != actual.PropertyInfo)
            {
                throw new AssertionException($"Expected property {expected.PropertyInfo.Name} but was {actual.PropertyInfo.Name}");
            }

            return true;
        }
    }
}