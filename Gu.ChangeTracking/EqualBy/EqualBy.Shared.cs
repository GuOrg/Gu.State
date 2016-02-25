namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;

    public static partial class EqualBy
    {
        private static bool EnumerableEquals<TSetting>(object x, object y, Func<object, object, TSetting, bool> compareItem, TSetting settings)
            where TSetting : IEqualBySettings
        {
            if (settings.ReferenceHandling == ReferenceHandling.Throw)
            {
                // using ensure to throw
                Ensure.NotIs<IEnumerable>(x, nameof(x));
            }

            var xlist = x as IList;
            var ylist = y as IList;
            if (xlist != null && ylist != null)
            {
                if (xlist.Count != ylist.Count)
                {
                    return false;
                }

                for (int i = 0; i < xlist.Count; i++)
                {
                    var xv = xlist[i];
                    var yv = ylist[i];

                    if (!compareItem(xv, yv, settings))
                    {
                        return false;
                    }
                }
            }
            else
            {
                // using ensure to throw
                Ensure.NotIs<IEnumerable>(x, nameof(x));
            }

            return true;
        }

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
