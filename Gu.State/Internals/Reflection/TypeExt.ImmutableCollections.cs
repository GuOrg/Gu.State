namespace Gu.State
{
    using System;

    internal static partial class TypeExt
    {
        internal static bool IsInSystemCollections(this Type type)
        {
            return type.FullName.StartsWith("System.Collections");
        }
    }
}