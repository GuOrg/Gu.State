namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class TestHelpers
    {
        public static T GetFieldValue<T>(this object source, string fieldName)
        {
            var fieldInfo = source.GetType().GetField(fieldName, Constants.DefaultFieldBindingFlags);
            return (T)fieldInfo.GetValue(source);
        }

        public static PropertyInfo GetProperty(this object item, string propertyName)
        {
            var propertyInfo = item.GetType()
                                   .GetProperty(propertyName);
            if (propertyInfo is null)
            {
                throw new ArgumentOutOfRangeException($"The type: {item.GetType().Name} does not have a property named: {propertyName}");
            }

            return propertyInfo;
        }

        public static void Add<T>(this List<T> list, T item1, T item2)
        {
            list.Add(item1);
            list.Add(item2);
        }
    }
}
