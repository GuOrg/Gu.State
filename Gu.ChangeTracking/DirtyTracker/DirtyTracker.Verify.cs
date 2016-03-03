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
        /// <param name="bindingFlags">
        /// The binding flags to use when getting properties
        /// Default is BindingFlags.Instance | BindingFlags.Public
        /// </param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        public static void VerifyCanTrack<T>(
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw)
            where T : class, INotifyPropertyChanged
        {
            var settings = PropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
            VerifyCanTrack<T>(settings);
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be tracked.
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to track</typeparam>
        /// <param name="settings">Contains configuration for how tracking is performed</param>
        public static void VerifyCanTrack<T>(PropertiesSettings settings) 
            where T : class, INotifyPropertyChanged
        {
            VerifyCanTrack(typeof(T), settings);
        }

        /// <summary>
        /// Check if the properties of <paramref name="type"/> can be tracked.
        /// This method will throw an exception if copy cannot be performed for <paramref name="type"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <param name="type">The type to track</param>
        /// <param name="settings">Contains configuration for how tracking is performed</param>
        public static void VerifyCanTrack(Type type, PropertiesSettings settings)
        {
            ChangeTracker.VerifyCanTrack(type, settings);
        }
    }
}
