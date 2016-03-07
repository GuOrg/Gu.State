namespace Gu.State
{
    using System;
    using System.Reflection;

    public static partial class Synchronize
    {
        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be synchronized.
        /// This method will throw an exception if synchronization cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to verify that synchronization is possible for</typeparam>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        public static void VerifyCanSynchronize<T>(
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw)
        {
            var settings = PropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
            VerifyCanSynchronize<T>(settings);
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be synchronized.
        /// This method will throw an exception if synchronization cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <param name="settings">Contains configuration for how synchronization will be performed</param>
        public static void VerifyCanSynchronize<T>(PropertiesSettings settings)
        {
            VerifyCanSynchronize(typeof(T), settings);
        }

        /// <summary>
        /// Check if the properties of <paramref name="type"/> can be synchronized.
        /// This method will throw an exception if synchronization cannot be performed for <paramref name="type"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="settings">Contains configuration for how synchronization will be performed</param>
        public static void VerifyCanSynchronize(Type type, PropertiesSettings settings)
        {
            Track.VerifyCanTrackChanges(type, settings);
            Copy.VerifyCanCopyPropertyValues(type, settings);
        }
    }
}
