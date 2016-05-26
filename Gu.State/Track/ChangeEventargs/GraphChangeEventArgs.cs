namespace Gu.State
{
    using System.Diagnostics;
    using System.Reflection;

    public static class GraphChangeEventArgs
    {
        internal static PropertyGraphChangedEventArgs<T> Create<T>(T root, PropertyInfo property)
        {
            Debug.Assert(property != null, "property == null");
            return new PropertyGraphChangedEventArgs<T>(root, property);
        }

        internal static ItemGraphChangedEventArgs<T> Create<T>(T root, int index)
        {
            return new ItemGraphChangedEventArgs<T>(root, index);
        }
    }
}