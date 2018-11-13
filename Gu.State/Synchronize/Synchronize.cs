namespace Gu.State
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    /// <summary>Exposing methods for keeping two items synchronized.</summary>
    public static partial class Synchronize
    {
        /// <summary>
        /// Synchronizes property values from source to target.
        /// </summary>
        /// <typeparam name="T">The type os <paramref name="source"/> and <paramref name="target"/>.</typeparam>
        /// <param name="source">The instance to copy property values from.</param>
        /// <param name="target">The instance to copy property values to.</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is used to new up references so a default constructor is required, can be private.
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting properties.</param>
        /// <returns>A disposable that when disposed stops synchronizing.</returns>
        public static IDisposable PropertyValues<T>(
            T source,
            T target,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags)
            where T : class, INotifyPropertyChanged
        {
            var settings = PropertiesSettings.GetOrCreate(referenceHandling, bindingFlags);
            return PropertyValues(source, target, settings);
        }

        /// <summary>
        /// Synchronizes property values from source to target.
        /// </summary>
        /// <typeparam name="T">The type os <paramref name="source"/> and <paramref name="target"/>.</typeparam>
        /// <param name="source">The instance to copy property values from.</param>
        /// <param name="target">The instance to copy property values to.</param>
        /// <param name="settings">Contains configuration for how synchronization will be performed.</param>
        /// <returns>A disposable that when disposed stops synchronizing.</returns>
        public static IDisposable PropertyValues<T>(T source, T target, PropertiesSettings settings)
            where T : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));
            Ensure.NotSame(source, target, nameof(source), nameof(target));
            Ensure.SameType(source, target);
            VerifyCanSynchronize(source.GetType(), settings, typeof(Synchronize).Name, nameof(PropertyValues));
            return TrackerCache.GetOrAdd(source, target, settings, pair => new Synchronizer(source, target, settings));
        }
    }
}