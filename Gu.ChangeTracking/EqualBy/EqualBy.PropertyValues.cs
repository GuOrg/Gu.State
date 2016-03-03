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
        /// <typeparam name="T">The type to get ignore properties for settings for</typeparam>
        /// <param name="x">The first instance</param>
        /// <param name="y">The second instance</param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used a deep equals is performed.
        /// Default value is Throw
        /// </param>
        /// <param name="excludedProperties">Names of properties on <typeparamref name="T"/> to exclude from copying</param>
        /// <returns>True if <paramref name="x"/> and <paramref name="y"/> are equal</returns>
        public static bool PropertyValues<T>(
            T x,
            T y,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw)
        {
            var settings = PropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
            return PropertyValues(x, y, settings);
        }

        /// <summary>
        /// Compares x and y for equality using property values.
        /// If a type implements IList the items of the list are compared
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="x"/> and <paramref name="y"/></typeparam>
        /// <param name="x">The first instance</param>
        /// <param name="y">The second instance</param>
        /// <param name="settings">Specifies how equality is performed.</param>
        /// <returns>True if <paramref name="x"/> and <paramref name="y"/> are equal</returns>
        public static bool PropertyValues<T>(T x, T y, PropertiesSettings settings)
        {
            var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
            ErrorBuilder.Start()
                        .OnlyValidProperties(type, settings, IsPropertyValid)
                        .OnlySupportedIndexers(type, settings)
                        .HasReferenceHandlingIfEnumerable(type, settings)
                        .ThrowIfHasErrors(type, settings);

            if (settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops)
            {
                var referencePairs = new ReferencePairCollection();
                return PropertiesValuesEquals(x, y, settings, referencePairs);
            }

            return PropertiesValuesEquals(x, y, settings, null);
        }

        private static bool PropertiesValuesEquals<T>(
            T x,
            T y,
            PropertiesSettings settings,
            ReferencePairCollection referencePairs)
        {
            var type = x?.GetType() ?? y?.GetType();
            ErrorBuilder.Start()
                        .OnlySupportedIndexers(type, settings)
                        .ThrowIfHasErrors<PropertiesSettings>(type, settings);

            referencePairs?.Add(x, y);
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

            if (x.GetType().IsEquatable())
            {
                return Equals(x, y);
            }

            if (x is IEnumerable)
            {
                if (!EnumerableEquals(x, y, ItemPropertiesEquals, settings, referencePairs))
                {
                    return false;
                }
            }

            var propertyInfos = x.GetType()
                                 .GetProperties(settings.BindingFlags);
            foreach (var propertyInfo in propertyInfos)
            {
                if (settings.IsIgnoringProperty(propertyInfo))
                {
                    continue;
                }

                var xv = propertyInfo.GetValue(x);
                var yv = propertyInfo.GetValue(y);
                if (referencePairs?.Contains(xv, yv) == true)
                {
                    continue;
                }

                if (!PropertyValueEquals(xv, yv, propertyInfo, settings, referencePairs))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ItemPropertiesEquals(
            object x,
            object y,
            PropertiesSettings settings,
            ReferencePairCollection referencePairs)
        {
            return PropertyValueEquals(x, y, null, settings, referencePairs);
        }

        private static bool PropertyValueEquals(
            object x,
            object y,
            PropertyInfo propertyInfo,
            PropertiesSettings settings,
            ReferencePairCollection referencePairs)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.GetType().IsEquatable())
            {
                return Equals(x, y);
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References:
                    return ReferenceEquals(x, y);
                case ReferenceHandling.Structural:
                case ReferenceHandling.StructuralWithReferenceLoops:
                    return PropertiesValuesEquals(x, y, settings, referencePairs);
                case ReferenceHandling.Throw:
                    Throw.CannotCompareMember<PropertiesSettings>(x.GetType(), propertyInfo);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(settings.ReferenceHandling),
                        settings.ReferenceHandling,
                        null);
            }

            return true;
        }
    }
}
