namespace Gu.State
{
    using System.Reflection;

    /// <summary>
    /// Methods for checking if diffing will work for types.
    /// </summary>
    public static partial class DiffBy
    {
        /// <summary>
        /// Check if the fields of <typeparamref name="T"/> can be compared for equality
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for.</typeparam>
        /// <param name="referenceHandling">
        /// If Structural is used a deep equality check is performed.
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting fields.</param>
        public static void VerifyCanDiffByFieldValues<T>(
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags)
        {
            var settings = FieldsSettings.GetOrCreate(referenceHandling, bindingFlags);
            VerifyCanDiffByFieldValues<T>(settings);
        }

        /// <summary>
        /// Check if the fields of <typeparamref name="T"/> can be compared for equality
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <param name="settings">The settings to use.</param>
        public static void VerifyCanDiffByFieldValues<T>(FieldsSettings settings)
        {
            if (settings is null)
            {
                throw new System.ArgumentNullException(nameof(settings));
            }

            EqualBy.VerifyCanEqualByMemberValues(typeof(T), settings, typeof(DiffBy).Name, nameof(FieldValues));
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be compared for equality
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore properties for settings for.</typeparam>
        /// <param name="referenceHandling">
        /// If Structural is used a deep equality check is performed.
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting properties.</param>
        public static void VerifyCanDiffByPropertyValues<T>(
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags)
        {
            var settings = PropertiesSettings.GetOrCreate(referenceHandling, bindingFlags);
            VerifyCanDiffByPropertyValues<T>(settings);
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be compared for equality
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <param name="settings">The settings to use.</param>
        public static void VerifyCanDiffByPropertyValues<T>(PropertiesSettings settings)
        {
            if (settings is null)
            {
                throw new System.ArgumentNullException(nameof(settings));
            }

            EqualBy.VerifyCanEqualByMemberValues(typeof(T), settings, typeof(DiffBy).Name, nameof(PropertyValues));
        }
    }
}
