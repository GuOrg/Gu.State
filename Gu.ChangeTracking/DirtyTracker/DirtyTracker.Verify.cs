namespace Gu.ChangeTracking
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    public static partial class DirtyTracker
    {
        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be tracked.
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore properties for settings for</typeparam>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        /// <param name="ignoreProperties">Names of properties on <typeparamref name="T"/> to exclude from copying</param>
        public static void VerifyCanTrack<T>(
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw,
            params string[] ignoreProperties)
            where T : class, INotifyPropertyChanged
        {
            var settings = PropertiesSettings.Create(typeof(T), bindingFlags, referenceHandling, ignoreProperties);
            VerifyCanTrack<T>(settings);
        }

        public static void VerifyCanTrack<T>(PropertiesSettings settings)
                        where T : class, INotifyPropertyChanged
        {
            VerifyCanTrack(typeof(T), settings);
        }

        public static void VerifyCanTrack(Type type, PropertiesSettings settings)
        {
            ChangeTracker.VerifyCanTrack(type, settings);
        }
    }
}
