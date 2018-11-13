namespace Gu.State
{
    using System.Reflection;

    public static partial class Copy
    {
        /// <summary>
        /// Copies property values from source to target.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="source" /> and <paramref name="target" />.</typeparam>
        /// <param name="source">The instance to copy property values from.</param>
        /// <param name="target">The instance to copy property values to.</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private.
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting properties.</param>
        public static void PropertyValues<T>(
            T source,
            T target,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags)
            where T : class
        {
            var settings = PropertiesSettings.GetOrCreate(referenceHandling, bindingFlags);
            PropertyValues(source, target, settings);
        }

        /// <summary>
        /// Copies property values from source to target.
        /// </summary>
        /// <typeparam name="T">The type to to copy.</typeparam>
        /// <param name="source">The instance to copy property values from.</param>
        /// <param name="target">The instance to copy property values to.</param>
        /// <param name="settings">Contains configuration for how copy will be performed.</param>
        public static void PropertyValues<T>(T source, T target, PropertiesSettings settings)
            where T : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));
            Ensure.SameType(source, target, nameof(source), nameof(target));
            Verify.CanCopyRoot(typeof(T), settings);
            Sync(source, target, settings);
        }
    }
}
