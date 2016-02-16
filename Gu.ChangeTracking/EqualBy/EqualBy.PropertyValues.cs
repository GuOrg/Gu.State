namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;

    public static partial class EqualBy
    {
        /// <summary>
        /// Compares x and y for equality using property values.
        /// If a type implements IList the items of the list are compared
        /// </summary>
        /// <param name="referenceHandling">
        /// Specifies how reference types are compared.
        /// Structural compares property values recursively.
        /// </param>
        public static bool PropertyValues<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return PropertyValues(x, y, Constants.DefaultPropertyBindingFlags, referenceHandling);
        }

        /// <summary>
        /// Compares x and y for equality using property values.
        /// If a type implements IList the items of the list are compared
        /// </summary>
        /// <param name="referenceHandling">
        /// Specifies how reference types are compared.
        /// Structural compares property values recursively.
        /// </param>
        public static bool PropertyValues<T>(T x, T y, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            Ensure.SameType(x, y, nameof(x), nameof(y));
            return PropertyValuesCore(x, y, bindingFlags, referenceHandling);
        }

        public static bool PropertyValues<T>(T x, T y, params string[] excludedProperties)
        {
            return PropertyValues(x, y, Constants.DefaultPropertyBindingFlags, excludedProperties);
        }

        public static bool PropertyValues<T>(T x, T y, BindingFlags bindingFlags, params string[] excludedProperties)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            Ensure.SameType(x, y, nameof(x), nameof(y));
            Ensure.NotIs<IEnumerable>(x, nameof(x));
            var propertyInfos = x.GetType().GetProperties(bindingFlags);
            foreach (var propertyInfo in propertyInfos)
            {
                if (excludedProperties?.Contains(propertyInfo.Name) == true)
                {
                    continue;
                }

                if (!IsEquatable(propertyInfo.PropertyType))
                {
                    var message = $"Copy does not support comparing the property {propertyInfo.Name} of type {propertyInfo.PropertyType}";
                    throw new NotSupportedException(message);
                }

                var xv = propertyInfo.GetValue(x);
                var yv = propertyInfo.GetValue(y);
                if (!Equals(xv, yv))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool PropertyValuesCore(object x, object y, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            if (x is IEnumerable)
            {
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

                        if (!PropertyValueEquals(xv, yv, bindingFlags, referenceHandling))
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
            }

            var propertyInfos = x.GetType().GetProperties(bindingFlags);
            foreach (var propertyInfo in propertyInfos)
            {
                var xv = propertyInfo.GetValue(x);
                var yv = propertyInfo.GetValue(y);
                if (ReferenceEquals(x, xv) && ReferenceEquals(y, yv))
                {
                    continue;
                }

                if (!PropertyValueEquals(xv, yv, bindingFlags, referenceHandling))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool PropertyValueEquals(object x, object y, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (IsEquatable(x.GetType()))
            {
                if (!Equals(x, y))
                {
                    return false;
                }
            }
            else
            {
                switch (referenceHandling)
                {
                    case ReferenceHandling.Reference:
                        if (ReferenceEquals(x, y))
                        {
                            return true;
                        }

                        return false;
                    case ReferenceHandling.Structural:
                        if (PropertyValuesCore(x, y, bindingFlags, referenceHandling))
                        {
                            return true;
                        }

                        return false;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(referenceHandling), referenceHandling, null);
                }
            }

            return true;
        }
    }
}
