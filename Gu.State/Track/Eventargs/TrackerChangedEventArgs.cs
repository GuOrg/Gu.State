namespace Gu.State
{
    using System.Reflection;

    internal static class TrackerChangedEventArgs
    {
        internal static PropertyGraphChangedEventArgs<T> Create<T>(T root, PropertyInfo property)
        {
            return new PropertyGraphChangedEventArgs<T>(root, property);
        }

        internal static ItemGraphChangedEventArgs<T> Create<T>(T root, int index)
        {
            return new ItemGraphChangedEventArgs<T>(root, index);
        }
    }
}