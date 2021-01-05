namespace Gu.State
{
    using System.Reflection;

    /// <summary>
    /// Methods for diffing by fields.
    /// </summary>
    public static partial class DiffBy
    {
        /// <summary>
        /// Compares x and y for equality using field values.
        /// If a type implements IList the items of the list are compared.
        /// </summary>
        /// <typeparam name="T">The type to compare.</typeparam>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The second instance.</param>
        /// <param name="referenceHandling">
        /// If Structural is used a deep equals is performed.
        /// Default value is Throw.
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting properties.</param>
        /// <returns>Diff.Empty if <paramref name="x"/> and <paramref name="y"/> are equal.</returns>
        public static Diff FieldValues<T>(
            T x,
            T y,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags)
        {
            var settings = FieldsSettings.GetOrCreate(referenceHandling, bindingFlags);
            return FieldValues(x, y, settings);
        }

        /// <summary>
        /// Compares x and y for equality using field values and returns the difference.
        /// If a type implements IList the items of the list are compared.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="x"/> and <paramref name="y"/>.</typeparam>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The second instance.</param>
        /// <param name="settings">Specifies how equality is performed.</param>
        /// <returns>Diff.Empty if <paramref name="x"/> and <paramref name="y"/> are equal.</returns>
        public static Diff FieldValues<T>(T x, T y, FieldsSettings settings)
        {
            if (x is null)
            {
                throw new System.ArgumentNullException(nameof(x));
            }

            if (y is null)
            {
                throw new System.ArgumentNullException(nameof(y));
            }

            if (settings is null)
            {
                throw new System.ArgumentNullException(nameof(settings));
            }

            EqualBy.VerifyCanEqualByMemberValues(x.GetType(), settings, nameof(DiffBy), nameof(FieldValues));
            return TryCreateValueDiff(x, y, settings) ?? new EmptyDiff(x, y);
        }
    }
}
