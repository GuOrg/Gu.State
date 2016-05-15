namespace Gu.State.Tests
{
    public class PropertyChangedEventArgsComparer : EventArgsComparer<PropertyChangeEventArgs>
    {
        public static readonly PropertyChangedEventArgsComparer Default = new PropertyChangedEventArgsComparer();

        public override bool Equals(PropertyChangeEventArgs x, PropertyChangeEventArgs y)
        {
            return x.PropertyInfo == y.PropertyInfo;
        }
    }
}