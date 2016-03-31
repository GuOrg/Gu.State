namespace Gu.State.Tests
{
    using System.Runtime.Remoting.Contexts;
    using System.Security.Cryptography.X509Certificates;

    public static class TestHelpers
    {
        public static T GetFieldValue<T>(this object source, string fieldName)
        {
            var fieldInfo = source.GetType().GetField(fieldName, Constants.DefaultFieldBindingFlags);
            return (T)fieldInfo.GetValue(source);
        }
    }
}
