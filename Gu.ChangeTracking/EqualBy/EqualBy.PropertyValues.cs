namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
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
            return PropertyValues(x, y, new EqualByPropertiesSettings(null, bindingFlags, referenceHandling));
        }

        public static bool PropertyValues<T>(T x, T y, params string[] excludedProperties)
        {
            return PropertyValues(x, y, Constants.DefaultPropertyBindingFlags, excludedProperties);
        }

        public static bool PropertyValues<T>(T x, T y, BindingFlags bindingFlags, params string[] excludedProperties)
        {
            var settings = new EqualByPropertiesSettings(x?.GetType().GetIgnoreProperties(bindingFlags, excludedProperties), bindingFlags, ReferenceHandling.Throw);
            return PropertyValues(x, y, settings);
        }

        public static bool PropertyValues(object x, object y, EqualByPropertiesSettings settings)
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
                if (!ListEquals(x, y, PropertyItemEquals, settings))
                {
                    return false;
                }
            }

            var propertyInfos = x.GetType().GetProperties(settings.BindingFlags);
            foreach (var propertyInfo in propertyInfos)
            {
                if (settings.IsIgnoringProperty(propertyInfo))
                {
                    continue;
                }

                if (!IsEquatable(propertyInfo.PropertyType) && settings.ReferenceHandling == ReferenceHandling.Throw)
                {
                    var message = $"Copy does not support comparing the property {propertyInfo.Name} of type {propertyInfo.PropertyType}";
                    throw new NotSupportedException(message);
                }

                var xv = propertyInfo.GetValue(x);
                var yv = propertyInfo.GetValue(y);
                if (ReferenceEquals(x, xv) && ReferenceEquals(y, yv))
                {
                    continue;
                }

                if (!PropertyValueEquals(xv, yv, settings))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool PropertyItemEquals(object x, object y, EqualByPropertiesSettings settings)
        {
            return PropertyValueEquals(x, y, settings);
        }

        private static bool PropertyValueEquals(object x, object y, EqualByPropertiesSettings settings)
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
                switch (settings.ReferenceHandling)
                {
                    case ReferenceHandling.Reference:
                        if (ReferenceEquals(x, y))
                        {
                            return true;
                        }

                        return false;
                    case ReferenceHandling.Structural:
                        if (PropertyValues(x, y, settings))
                        {
                            return true;
                        }

                        return false;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(settings.ReferenceHandling), settings.ReferenceHandling, null);
                }
            }

            return true;
        }
    }
}
