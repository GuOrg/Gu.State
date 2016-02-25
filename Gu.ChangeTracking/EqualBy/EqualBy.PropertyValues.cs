namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Text;

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
            var settings = EqualByPropertiesSettings.Create(x,y, bindingFlags, ReferenceHandling.Throw, excludedProperties);
            return PropertyValues(x, y, settings);
        }

        public static bool PropertyValues<T>(T x, T y, IEqualByPropertiesSettings settings)
        {
            if (settings.ReferenceHandling == ReferenceHandling.Throw)
            {
                var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    ThrowCannotComparePropertiesForType(type);
                }

                var properties = type.GetProperties(settings.BindingFlags);
                foreach (var propertyInfo in properties)
                {
                    if (settings.IsIgnoringProperty(propertyInfo))
                    {
                        continue;
                    }

                    if (!propertyInfo.PropertyType.IsEquatable())
                    {
                        ThrowCannotCompareProperty(type, propertyInfo);
                    }
                }
            }

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

            if (IsEquatable(x.GetType()))
            {
                return Equals(x, y);
            }

            if (x is IEnumerable)
            {
                if (!EnumerableEquals(x, y, PropertyItemEquals, settings))
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
                    ThrowCannotCompareProperty(x.GetType(), propertyInfo);
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

        private static bool PropertyItemEquals(object x, object y, IEqualByPropertiesSettings settings)
        {
            return PropertyValueEquals(x, y, settings);
        }

        private static bool PropertyValueEquals(object x, object y, IEqualByPropertiesSettings settings)
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
                    case ReferenceHandling.Throw:
                        ThrowCannotComparePropertiesForType(x.GetType());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(settings.ReferenceHandling), settings.ReferenceHandling, null);
                }
            }

            return true;
        }

        private static void ThrowCannotCompareProperty(Type type, PropertyInfo propertyInfo)
        {
            var errorBuilder = new StringBuilder();
            errorBuilder.AppendLine($"EqualBy.{nameof(PropertyValues)}(x, y) does not support comparing the property {type.PrettyName()}.{propertyInfo.Name} of type {propertyInfo.PropertyType.PrettyName()}.")
                        .AppendSolveTheProblemBy()
                        .AppendSuggestImplementIEquatable(propertyInfo.PropertyType)
                        .AppendSuggestEqualBySettings<EqualByFieldsSettings>();
            throw new NotSupportedException(errorBuilder.ToString());
        }

        private static void ThrowCannotComparePropertiesForType(Type type)
        {
            var errorBuilder = new StringBuilder();
            errorBuilder.AppendLine($"EqualBy.{nameof(PropertyValues)}(x, y) does not support comparing the type {type.PrettyName()}.")
                        .AppendSolveTheProblemBy()
                        .AppendSuggestImplementIEquatable(type)
                        .AppendSuggestEqualBySettings<EqualByPropertiesSettings>();
            throw new NotSupportedException(errorBuilder.ToString());
        }
    }
}
