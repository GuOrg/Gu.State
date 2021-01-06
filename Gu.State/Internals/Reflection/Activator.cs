namespace Gu.State
{
    using System;
    using System.Globalization;
    using System.Reflection;

    internal static class Activator
    {
        internal static T CreateInstance<T>(Type type, object[] args)
        {
            return (T)System.Activator.CreateInstance(
                type,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.CreateInstance,
                (Binder)null,
                args,
                (CultureInfo)null,
                (object[])null);
        }
    }
}
