namespace Gu.State
{
    using System.Reflection;

    internal static class Constants
    {
        internal const BindingFlags DefaultPropertyBindingFlags = BindingFlags.Instance | BindingFlags.Public;

        internal const BindingFlags DefaultFieldBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    }
}
