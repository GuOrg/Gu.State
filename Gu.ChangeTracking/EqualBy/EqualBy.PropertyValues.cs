namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Reflection;

    public static partial class EqualBy
    {
        public static bool PropertyValues<T>(T x, T y, BindingFlags bindingFlags)
        {
            var settings = EqualByPropertiesSettings.GetOrCreate(bindingFlags);
            return PropertyValues(x, y, settings);
        }

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
            var settings = EqualByPropertiesSettings.GetOrCreate(referenceHandling);
            return PropertyValues(x, y, settings);
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
            var settings = EqualByPropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
            return PropertyValues(x, y, settings);
        }

        public static bool PropertyValues<T>(T x, T y, params string[] excludedProperties)
        {
            return PropertyValues(x, y, Constants.DefaultPropertyBindingFlags, excludedProperties);
        }

        public static bool PropertyValues<T>(T x, T y, BindingFlags bindingFlags, params string[] excludedProperties)
        {
            var settings = EqualByPropertiesSettings.Create(x, y, bindingFlags, ReferenceHandling.Throw, excludedProperties);
            return PropertyValues(x, y, settings);
        }

        public static bool PropertyValues<T>(T x, T y, IEqualByPropertiesSettings settings)
        {
            Verify.PropertyTypes(x, y, settings);
            Verify.Enumerable(x, y, settings);
            return PropertiesValuesEquals(x, y, settings);
        }

        private static bool PropertiesValuesEquals<T>(T x, T y, IEqualByPropertiesSettings settings)
        {
            Verify.Indexers(x?.GetType() ?? y?.GetType(), settings);
            if (ReferenceEquals(x, y))
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

            if (IsEquatable(x.GetType()))
            {
                return Equals(x, y);
            }

            if (x is IEnumerable)
            {
                if (!EnumerableEquals(x, y, ItemPropertiesEquals, settings))
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

                var xv = propertyInfo.GetValue(x);
                var yv = propertyInfo.GetValue(y);
                if (!PropertyValueEquals(xv, yv, propertyInfo, settings))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ItemPropertiesEquals(object x, object y, IEqualByPropertiesSettings settings)
        {
            return PropertyValueEquals(x, y, null, settings);
        }

        private static bool PropertyValueEquals(object x, object y, PropertyInfo propertyInfo, IEqualByPropertiesSettings settings)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (IsEquatable(x.GetType()))
            {
                return Equals(x, y);
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References:
                    return ReferenceEquals(x, y);
                case ReferenceHandling.Structural:
                    return PropertiesValuesEquals(x, y, settings);
                case ReferenceHandling.Throw:
                    Throw.CannotCompareMember(x.GetType(), propertyInfo);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.ReferenceHandling), settings.ReferenceHandling, null);
            }

            return true;
        }
    }
}
