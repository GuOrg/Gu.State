namespace Gu.State
{
    using System.Reflection;

    /// <summary>
    /// Exposes methods for copying via fields.
    /// </summary>
    public static partial class Copy
    {
        /// <summary>
        /// Copies field values from source to target.
        /// Event fields are excluded.
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for.</typeparam>
        /// <param name="source">The instance to copy field values from.</param>
        /// <param name="target">The instance to copy field values to.</param>
        /// <param name="referenceHandling">
        /// If Structural is used field values for sub fields are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private.
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting properties.</param>
        public static void FieldValues<T>(
            T source,
            T target,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags)
            where T : class
        {
            if (source is null)
            {
                throw new System.ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new System.ArgumentNullException(nameof(target));
            }

            var settings = FieldsSettings.GetOrCreate(referenceHandling, bindingFlags);
            FieldValues(source, target, settings);
        }

        /// <summary>
        /// Copies field values from source to target.
        /// Event fields are excluded.
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for.</typeparam>
        /// <param name="source">The instance to copy field values from.</param>
        /// <param name="target">The instance to copy field values to.</param>
        /// <param name="settings">Contains configuration for how to copy.</param>
        public static void FieldValues<T>(T source, T target, FieldsSettings settings)
            where T : class
        {
            if (source is null)
            {
                throw new System.ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new System.ArgumentNullException(nameof(target));
            }

            if (settings is null)
            {
                throw new System.ArgumentNullException(nameof(settings));
            }

            Ensure.SameType(source, target, nameof(source), nameof(target));
            Verify.CanCopyRoot(typeof(T), settings);
            Sync(source, target, settings);
        }
    }
}
