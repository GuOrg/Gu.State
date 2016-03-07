namespace Gu.State
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    public static partial class Synchronizer
    {
        /// <summary>
        /// Synchronizes property values from source to target.
        /// </summary>
        /// <typeparam name="T">The type os <paramref name="source"/> and <param name="target"></param></typeparam>
        /// <param name="source">The instance to copy property values from</param>
        /// <param name="target">The instance to copy property values to</param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        /// <returns>A disposable that when disposed stops synchronizing</returns>
        public static IDisposable CreatePropertySynchronizer<T>(
            T source,
            T target,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw)
            where T : class, INotifyPropertyChanged
        {
            var settings = PropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
            return new PropertySynchronizer<T>(source, target, settings);
        }

        /// <summary>
        /// Synchronizes property values from source to target.
        /// </summary>
        /// <typeparam name="T">The type os <paramref name="source"/> and <param name="target"></param></typeparam>
        /// <param name="source">The instance to copy property values from</param>
        /// <param name="target">The instance to copy property values to</param>
        /// <param name="settings">Contains configuration for how synchronization will be performed</param>
        /// <returns>A disposable that when disposed stops synchronizing</returns>
        public static IDisposable CreatePropertySynchronizer<T>(T source, T target, PropertiesSettings settings)
            where T : class, INotifyPropertyChanged
        {
            return new PropertySynchronizer<T>(source, target, settings);
        }
    }
}