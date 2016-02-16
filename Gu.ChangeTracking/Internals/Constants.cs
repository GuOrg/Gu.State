namespace Gu.ChangeTracking
{
    using System.Reflection;

    internal static class Constants
    {
        internal static readonly string[] EmptyIgnoreProperties = new string[0];

        internal static readonly BindingFlags DefaultPropertyBindingFlags = BindingFlags.Instance | BindingFlags.Public;

        internal static readonly BindingFlags DefaultFieldBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    }
}
