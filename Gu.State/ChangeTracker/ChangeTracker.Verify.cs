namespace Gu.State
{
    using System;
    using System.Reflection;

    public partial class ChangeTracker
    {
        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be tracked.
        /// This method will throw an exception if synchronization cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is used to new up references so a default constructor is required, can be private
        /// </param>
        public static void VerifyCanTrack<T>(
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw)
        {
            var settings = PropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
            VerifyCanTrack<T>(settings);
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be tracked.
        /// This method will throw an exception if synchronization cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <param name="settings">Contains configuration for how tracking will be performed</param>
        public static void VerifyCanTrack<T>(PropertiesSettings settings)
        {
            VerifyCanTrack(typeof(T), settings);
        }

        /// <summary>
        /// Check if the properties of <paramref name="type"/> can be tracked.
        /// This method will throw an exception if trackling cannot be performed for <paramref name="type"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="settings">Contains configuration for how tracking will be performed</param>
        public static void VerifyCanTrack(Type type, PropertiesSettings settings)
        {
            Verify.IsTrackableType(type, settings);
        }

        /// <summary>
        /// This class is used for failing fast and throwing with nice exception messages.
        /// </summary>
        private static class Verify
        {
            internal static void IsTrackableType(Type type, ChangeTracker tracker)
            {
                var errors = GetErrors(type, tracker.Settings, tracker.Path);
                if (errors != null)
                {
                    Throw.IfHasErrors(errors, tracker.Settings);
                }
            }

            internal static void IsTrackableType(Type type, PropertiesSettings settings)
            {
                var errors = GetErrors(type, settings, null);
                if (errors != null)
                {
                    Throw.IfHasErrors(errors, settings);
                }
            }

            internal static void IsTrackablePropertyValue(Type propertyValueType, PropertyInfo propertyInfo, ChangeTracker tracker)
            {
                var path = tracker.Path.WithProperty(propertyInfo);
                var errors = GetErrors(propertyValueType, tracker.Settings, path);
                if (errors != null)
                {
                    Throw.IfHasErrors(errors, tracker.Settings);
                }
            }

            internal static void IsTrackableItemValue(Type itemType, int? index, ChangeTracker tracker)
            {
                var path = tracker.Path.WithIndex(index);
                var errors = GetErrors(itemType, tracker.Settings, path);
                if (errors != null)
                {
                    Throw.IfHasErrors(errors, tracker.Settings);
                }
            }

            private static TypeErrors GetErrors(Type type, PropertiesSettings settings, MemberPath path = null)
            {
                return settings.TrackableErrors.GetOrAdd(
                    type,
                    t => ErrorBuilder.Start()
                                     .CheckReferenceHandling(type, settings)
                                     .CheckIndexers(type, settings)
                                     .CheckNotifies(type, settings)
                                     .VerifyRecursive(t, settings, path, GetRecursiveErrors));
            }

            private static Error GetRecursiveErrors(PropertiesSettings settings, MemberPath path)
            {
                var type = path.LastNodeType;
                if (type.IsImmutable())
                {
                    return null;
                }

                if (settings.ReferenceHandling == ReferenceHandling.References)
                {
                    return null;
                }

                return GetErrors(type, settings, path);
            }
        }
    }
}
