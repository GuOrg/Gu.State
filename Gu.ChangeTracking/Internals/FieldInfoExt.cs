namespace Gu.ChangeTracking
{
    using System;
    using System.Reflection;

    internal static class FieldInfoExt
    {
        internal static bool IsEventField(this FieldInfo field)
        {
            return typeof(MulticastDelegate).IsAssignableFrom(field.FieldType);
        }
    }
}