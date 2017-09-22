namespace Gu.State
{
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>Provides methods for comparing tow instances.</summary>
    public static partial class DiffBy
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
        /// <returns>Diff.Empty if <paramref name="x"/> and <paramref name="y"/> are equal</returns>
        public static Diff PropertyValues<T>(
            T x,
            T y,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags)
        {
            var settings = PropertiesSettings.GetOrCreate(referenceHandling, bindingFlags);
            return PropertyValues(x, y, settings);
        }

        /// <summary>
        /// Compares x and y for equality using property values and returns the difference.
        /// If a type implements IList the items of the list are compared
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="x"/> and <paramref name="y"/></typeparam>
        /// <param name="x">The first instance</param>
        /// <param name="y">The second instance</param>
        /// <param name="settings">Specifies how equality is performed.</param>
        /// <returns>Diff.Empty if <paramref name="x"/> and <paramref name="y"/> are equal</returns>
        public static ValueDiff PropertyValues<T>(T x, T y, PropertiesSettings settings)
        {
            Ensure.NotNull(x, nameof(x));
            Ensure.NotNull(y, nameof(y));
            Ensure.NotNull(settings, nameof(settings));
            EqualBy.Verify.CanEqualByMemberValues(x, y, settings, typeof(DiffBy).Name, nameof(PropertyValues));
            return TryCreateValueDiff(x, y, settings) ?? new EmptyDiff(x, y);
        }

        internal static ValueDiff PropertyValuesOrNull<T>(T x, T y, PropertiesSettings settings)
        {
            Debug.Assert(settings != null, "settings == null");
            if (TryGetValueDiff(x, y, settings, out var diff))
            {
                return diff;
            }

            return TryCreateValueDiff(x, y, settings);
        }
    }
}
