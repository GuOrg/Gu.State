namespace Gu.State.Tests
{
    using System.Collections.Generic;

    public static class TestHelpers
    {
        public static T GetFieldValue<T>(this object source, string fieldName)
        {
            var fieldInfo = source.GetType().GetField(fieldName, Constants.DefaultFieldBindingFlags);
            return (T)fieldInfo.GetValue(source);
        }

        public static void Add<T>(this List<T> list, T item1, T item2)
        {
            list.Add(item1);
            list.Add(item2);
        }
    }
}
