namespace Gu.State
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static partial class Track
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
        public static void VerifyCanTrackIsDirty<T>(
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw)
            where T : class, INotifyPropertyChanged
        {
            var settings = PropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
            VerifyCanTrackIsDirty<T>(settings);
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be tracked.
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to track</typeparam>
        /// <param name="settings">Contains configuration for how tracking is performed</param>
        public static void VerifyCanTrackIsDirty<T>(PropertiesSettings settings)
            where T : class, INotifyPropertyChanged
        {
            VerifyCanTrackIsDirty(typeof(T), settings);
        }

        /// <summary>
        /// Check if the properties of <paramref name="type"/> can be tracked.
        /// This method will throw an exception if copy cannot be performed for <paramref name="type"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <param name="type">The type to track</param>
        /// <param name="settings">Contains configuration for how tracking is performed</param>
        public static void VerifyCanTrackIsDirty(Type type, PropertiesSettings settings)
        {
            EqualBy.VerifyCanEqualByPropertyValues(type, settings);
            VerifyCanTrackChanges(type, settings);
        }

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
        public static void VerifyCanTrackChanges<T>(
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw)
        {
            var settings = PropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
            VerifyCanTrackChanges<T>(settings);
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be tracked.
        /// This method will throw an exception if synchronization cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <param name="settings">Contains configuration for how tracking will be performed</param>
        public static void VerifyCanTrackChanges<T>(PropertiesSettings settings)
        {
            VerifyCanTrackChanges(typeof(T), settings);
        }

        /// <summary>
        /// Check if the properties of <paramref name="type"/> can be tracked.
        /// This method will throw an exception if trackling cannot be performed for <paramref name="type"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="settings">Contains configuration for how tracking will be performed</param>
        public static void VerifyCanTrackChanges(Type type, PropertiesSettings settings)
        {
            Verify.IsTrackableType(type, settings);
        }

        /// <summary>
        /// This class is used for failing fast and throwing with nice exception messages.
        /// </summary>
        internal static class Verify
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
                    var typeErrors = new TypeErrors(propertyInfo.DeclaringType, errors);
                    Throw.IfHasErrors(typeErrors, tracker.Settings);
                }
            }

            internal static void IsTrackableItemValue(Type collectionType, Type itemType, int? index, ChangeTracker tracker)
            {
                var path = tracker.Path.WithIndex(index);
                var errors = GetErrors(itemType, tracker.Settings, path);
                if (errors != null)
                {
                    var typeErrors = new TypeErrors(collectionType, errors);
                    Throw.IfHasErrors(typeErrors, tracker.Settings);
                }
            }

            private static TypeErrors GetErrors(Type type, PropertiesSettings settings, MemberPath path = null)
            {
                return settings.TrackableErrors.GetOrAdd(
                    type,
                    t => ErrorBuilder.Start()
                                     .CheckReferenceHandling(type, settings, x => !x.IsImmutable())
                                     .CheckIndexers(type, settings)
                                     .CheckNotifies(type, settings)
                                     .VerifyRecursive(t, settings, path, GetRecursiveErrors)
                                     .Finnish());
            }

            private static TypeErrors GetRecursiveErrors(PropertiesSettings settings, MemberPath path)
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

        private static class Throw
        {
            // ReSharper disable once UnusedParameter.Local
            internal static void IfHasErrors<TSetting>(TypeErrors errors, TSetting settings)
                where TSetting : class, IMemberSettings
            {
                if (errors == null)
                {
                    return;
                }

                if (errors.Errors.Count == 1 && ReferenceEquals(errors.Errors.Single(), RequiresReferenceHandling.Other))
                {
                    return;
                }

                var message = GetErrorText(errors, settings);
                throw new NotSupportedException(message);
            }

            // ReSharper disable once UnusedParameter.Local
            internal static string GetErrorText<TSettings>(TypeErrors errors, TSettings settings)
                where TSettings : class, IMemberSettings
            {
                var errorBuilder = new StringBuilder();
                errorBuilder.AppendLine("Track changes failed.")
                            .AppendNotSupported(errors)
                            .AppendSolveTheProblemBy()
                            .AppendSuggestNotify(errors)
                            .AppendSuggestImmutable(errors)
                            .AppendSuggestResizableCollection(errors)
                            .AppendSuggestDefaultCtor(errors)
                            .AppendLine($"* Use {typeof(TSettings).Name} and specify how change tracking is performed:")
                            .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Structural)} means that a the entire graph is tracked.")
                            .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.StructuralWithReferenceLoops)} same as Structural but handles reference loops.")
                            .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.References)} means that only the root level changes are tracked.")
                            .AppendSuggestExclude(errors);

                var message = errorBuilder.ToString();
                return message;
            }
        }
    }
}
