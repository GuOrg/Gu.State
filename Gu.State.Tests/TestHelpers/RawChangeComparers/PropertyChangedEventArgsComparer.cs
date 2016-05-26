namespace Gu.State.Tests
{
    using NUnit.Framework;

    public class PropertyChangedEventArgsComparer : EventArgsComparer<PropertyChangeEventArgs>
    {
        public static readonly PropertyChangedEventArgsComparer Default = new PropertyChangedEventArgsComparer();

        public override bool Equals(PropertyChangeEventArgs expected, PropertyChangeEventArgs actual)
        {
            if (expected.PropertyInfo != actual.PropertyInfo)
            {
                throw new AssertionException($"Expected property {expected.PropertyInfo.Name} but was {actual.PropertyInfo.Name}");
            }

            return true;
        }
    }
}