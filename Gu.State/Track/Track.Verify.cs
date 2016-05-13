namespace Gu.State
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Text;

    /// <summary>Provides methods for tracking stuff.</summary>
    public static partial class Track
    {
        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be tracked.
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore properties for settings for</typeparam>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        /// <param name="bindingFlags">
        /// The binding flags to use when getting properties
        /// Default is BindingFlags.Instance | BindingFlags.Public
        /// </param>
        public static void VerifyCanTrackIsDirty<T>(
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags)
            where T : class, INotifyPropertyChanged
        {
            var settings = PropertiesSettings.GetOrCreate(referenceHandling, bindingFlags);
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
            EqualBy.VerifyCanEqualByPropertyValues(type, settings, typeof(Track).Name, nameof(VerifyCanTrackIsDirty));
            VerifyCanTrackChanges(type, settings);
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be tracked.
        /// This method will throw an exception if synchronization cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is used to new up references so a default constructor is required, can be private
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        public static void VerifyCanTrackChanges<T>(
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags)
        {
            var settings = PropertiesSettings.GetOrCreate(referenceHandling, bindingFlags);
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
            VerifyCanTrackChanges(type, settings, typeof(Track).Name, nameof(VerifyCanTrackChanges));
        }

        public static void VerifyCanTrackChanges(Type type, PropertiesSettings settings, string className, string methodName)
        {
            Verify.IsTrackableType(type, settings, className, methodName);
        }

        internal static void VerifyCanTrackIsDirty(Type type, PropertiesSettings settings, string className, string methodName)
        {
            EqualBy.VerifyCanEqualByPropertyValues(type, settings, className, methodName);
            VerifyCanTrackChanges(type, settings, className, methodName);
        }

        private static bool HasErrors(this TypeErrors errors)
        {
            if (errors == null)
            {
                return false;
            }

            if (errors.Errors.Count == 1 && ReferenceEquals(errors.Errors[0], RequiresReferenceHandling.ComplexType))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// This class is used for failing fast and throwing with nice exception messages.
        /// </summary>
        internal static class Verify
        {
            internal static void IsTrackableValue(object value, PropertiesSettings settings)
            {
                var errors = GetErrors(value.GetType(), settings, null);
                if (errors != null)
                {
                    var typeErrors = new TypeErrors(null, errors);
                    Throw.IfHasErrors(typeErrors, settings, typeof(Track).Name, nameof(Track.Changes));
                }
            }

            internal static void IsTrackableType(Type type, PropertiesSettings settings, string className = null, string methodName = null)
            {
                var errors = GetErrors(type, settings, null);
                if (errors != null)
                {
                    Throw.IfHasErrors(errors, settings, className ?? typeof(Track).Name, methodName ?? nameof(Track.Changes));
                }
            }

            private static TypeErrors GetErrors(Type type, PropertiesSettings settings, MemberPath path = null)
            {
                return settings.TrackableErrors.GetOrAdd(
                    type,
                    t => ErrorBuilder.Start()
                                     .CheckRequiresReferenceHandling(type, settings, x => !settings.IsImmutable(x))
                                     .CheckIndexers(type, settings)
                                     .CheckNotifies(type, settings)
                                     .VerifyRecursive(t, settings, path, GetRecursiveErrors)
                                     .Finnish());
            }

            private static TypeErrors GetRecursiveErrors(PropertiesSettings settings, MemberPath path)
            {
                var type = path.LastNodeType;
                if (settings.IsImmutable(type))
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
            internal static void IfHasErrors<TSetting>(TypeErrors errors, TSetting settings, string className, string methodName)
                where TSetting : class, IMemberSettings
            {
                if (errors.HasErrors())
                {
                    var message = GetErrorText(errors, settings, className, methodName);
                    throw new NotSupportedException(message);
                }
            }

            // ReSharper disable once UnusedParameter.Local
            private static string GetErrorText<TSettings>(TypeErrors errors, TSettings settings, string className, string methodName)
                where TSettings : class, IMemberSettings
            {
                var errorBuilder = new StringBuilder();
                errorBuilder.AppendLine($"{className}.{methodName}(x, y) failed.")
                            .AppendNotSupported(errors)
                            .AppendSolveTheProblemBy()
                            .AppendSuggestNotify(errors)
                            .AppendSuggestImmutable(errors)
                            .AppendSuggestResizableCollection(errors)
                            .AppendSuggestDefaultCtor(errors)
                            .AppendLine($"* Use {typeof(TSettings).Name} and specify how change tracking is performed:")
                            .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Structural)} means that a the entire graph is tracked.")
                            .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.References)} means that only the root level changes are tracked.")
                            .AppendSuggestExclude(errors);

                var message = errorBuilder.ToString();
                return message;
            }
        }
    }
}
