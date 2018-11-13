namespace Gu.State
{
    using System.Reflection;

    public static partial class EqualBy
    {
        /// <summary>
        /// Compares x and y for equality using field values.
        /// If a type implements IList the items of the list are compared.
        /// Event fields are excluded.
        /// For performance the overload with settings should be used and the settings should be cached.
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for.</typeparam>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The second instance.</param>
        /// <param name="referenceHandling">
        /// If Structural is used a deep equals is performed.
        /// Default value is Throw.
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting fields.</param>
        /// <returns>True if <paramref name="x"/> and <paramref name="y"/> are equal.</returns>
        public static bool FieldValues<T>(
            T x,
            T y,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags)
        {
            var settings = FieldsSettings.GetOrCreate(referenceHandling, bindingFlags);
            return FieldValues(x, y, settings);
        }

        /// <summary>
        /// Compares x and y for equality using field values.
        /// If a type implements IList the items of the list are compared.
        /// Event fields are excluded.
        /// For performance the overload with settings should be used and the settings should be cached.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="x"/> and <paramref name="y"/>.</typeparam>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The second instance.</param>
        /// <param name="settings">Specifies how equality is performed.</param>
        /// <returns>True if <paramref name="x"/> and <paramref name="y"/> are equal.</returns>
        public static bool FieldValues<T>(T x, T y, FieldsSettings settings)
        {
            return MemberValues(x, y, settings);
        }
    }
}