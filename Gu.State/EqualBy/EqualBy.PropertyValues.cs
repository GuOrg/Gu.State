namespace Gu.State
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
        /// <typeparam name="T">The type to compare</typeparam>
        /// <param name="x">The first instance</param>
        /// <param name="y">The second instance</param>
        /// <param name="referenceHandling">
        /// If Structural is used a deep equals is performed.
        /// Default value is Throw
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <returns>True if <paramref name="x"/> and <paramref name="y"/> are equal</returns>
        public static bool PropertyValues<T>(
            T x,
            T y,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags)
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
            Verify.CanEqualByPropertyValues(x, y, settings);
            var pairs = settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops
                            ? new ReferencePairCollection()
                            : null;
            return PropertiesValuesEquals(x, y, settings, pairs);
        }

        private static bool PropertiesValuesEquals<T>(
            T x,
            T y,
            PropertiesSettings settings,
            ReferencePairCollection referencePairs)
        {
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

            if (settings.IsEquatable(x.GetType()))
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

            if (settings.IsEquatable(x.GetType()))
            {
                return Equals(x, y);
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References:
                    return ReferenceEquals(x, y);
                case ReferenceHandling.Structural:
                case ReferenceHandling.StructuralWithReferenceLoops:
                    Verify.CanEqualByPropertyValues(x, y, settings);
                    return PropertiesValuesEquals(x, y, settings, referencePairs);
                case ReferenceHandling.Throw:
                    throw Throw.ShouldNeverGetHereException();
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(settings.ReferenceHandling),
                        settings.ReferenceHandling,
                        null);
            }
        }
    }
}
