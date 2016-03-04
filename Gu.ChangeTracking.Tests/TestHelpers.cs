namespace Gu.ChangeTracking.Tests
{
    using NUnit.Framework;

    public static class TestHelpers
    {
        public static T GetFieldValue<T>(this object source, string fieldName)
        {
            var fieldInfo = source.GetType().GetField(fieldName, Constants.DefaultFieldBindingFlags);
            return (T)fieldInfo.GetValue(source);
        }
    }
}
