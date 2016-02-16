namespace Gu.ChangeTracking
{
    using System;

    public static partial class EqualBy
    {
        internal static bool IsEquatable(Type type)
        {
            if (type == typeof(string))
            {
                return true;
            }

            if (type.IsEnum)
            {
                return true;
            }

            if (type.IsNullable())
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                return IsEquatable(underlyingType);
            }

            return type.IsValueType && type.IsEquatable();
        }
    }
}
