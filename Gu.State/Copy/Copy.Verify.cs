namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Reflection;

    public static partial class Copy
    {
        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore properties for settings for</typeparam>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is used to new up references so a default constructor is required, can be private
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        public static void VerifyCanCopyPropertyValues<T>(
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags)
        {
            var settings = PropertiesSettings.GetOrCreate(referenceHandling, bindingFlags);
            VerifyCanCopyPropertyValues<T>(settings);
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <param name="settings">Contains configuration for how copy will be performed</param>
        public static void VerifyCanCopyPropertyValues<T>(PropertiesSettings settings)
        {
            var type = typeof(T);
            VerifyCanCopyPropertyValues(type, settings);
        }

        /// <summary>
        /// Check if the properties of <paramref name="type"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <paramref name="type"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="settings">Contains configuration for how copy will be performed</param>
        public static void VerifyCanCopyPropertyValues(Type type, PropertiesSettings settings)
        {
            Verify.CanCopyRoot(type, settings);
            Verify.CanCopyMemberValues(type, settings, typeof(Copy).Name, nameof(VerifyCanCopyPropertyValues));
        }

        /// <summary>
        /// Check if the fields of <typeparamref name="T"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for</typeparam>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting fields</param>
        public static void VerifyCanCopyFieldValues<T>(
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags)
        {
            var settings = FieldsSettings.GetOrCreate(referenceHandling, bindingFlags);
            VerifyCanCopyFieldValues<T>(settings);
        }

        /// <summary>
        /// Check if the fields of <typeparamref name="T"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for</typeparam>
        /// <param name="settings">Contains configuration for how copy is performed</param>
        public static void VerifyCanCopyFieldValues<T>(FieldsSettings settings)
        {
            var type = typeof(T);
            VerifyCanCopyFieldValues(type, settings);
        }

        /// <summary>
        /// Check if the fields of <paramref name="type"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <paramref name="type"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <param name="type">The type to get ignore fields for settings for</param>
        /// <param name="settings">Contains configuration for how copy is performed</param>
        public static void VerifyCanCopyFieldValues(Type type, FieldsSettings settings)
        {
            Verify.CanCopyRoot(type, settings);
            Verify.CanCopyMemberValues(type, settings, typeof(Copy).Name, nameof(VerifyCanCopyFieldValues));
        }

        internal static void VerifyCanCopyPropertyValues(Type type, PropertiesSettings settings, string classname, string methodName)
        {
            Verify.CanCopyRoot(type, settings);
            Verify.GetOrCreateErrors(type, settings)
                  .ThrowIfHasErrors(settings, classname, methodName);
        }

        private static ErrorBuilder.TypeErrorsBuilder CheckIsCopyableEnumerable(this ErrorBuilder.TypeErrorsBuilder typeErrors, Type type, MemberSettings settings)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(type))
            {
                return typeErrors;
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.Throw:
                case ReferenceHandling.References:
                    return typeErrors;
                case ReferenceHandling.Structural:
                    if (!IsCopyableCollectionType(type))
                    {
                        return typeErrors.CreateIfNull(type)
                                         .Add(new NotCopyableCollection(type));
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return typeErrors;
        }

        internal static class Verify
        {
            internal static void CanCopyMemberValues<T>(T x, T y, MemberSettings settings)
            {
                var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
                CanCopyMemberValues(type, settings, typeof(Copy).Name, settings.CopyMethodName());
            }

            internal static void CanCopyMemberValues(Type type, MemberSettings settings, string className, string methodName)
            {
                GetOrCreateErrors(type, settings)
                    .ThrowIfHasErrors(settings, className, methodName);
            }

            internal static void CanCopyRoot(Type type, MemberSettings settings)
            {
                if (settings.IsImmutable(type))
                {
                    throw new NotSupportedException("Cannot copy the members of an immutable object");
                }

                if (typeof(IEnumerable).IsAssignableFrom(type) && !IsCopyableCollectionType(type))
                {
                    throw new NotSupportedException($"Can only copy the members of collections implementing {typeof(IList).Name} or {typeof(IDictionary).Name}");
                }
            }

            internal static TypeErrors GetOrCreateErrors(Type type, MemberSettings settings, MemberPath path = null)
            {
                return settings.CopyErrors.GetOrAdd(type, t => CreateErrors(t, settings, path));
            }

            private static TypeErrors CreateErrors(Type type, MemberSettings settings, MemberPath path)
            {
                if (settings.IsImmutable(type) || settings.TryGetCopyer(type, out CustomCopy temp))
                {
                    return null;
                }

                var errors = VerifyCore(settings, type)
                    .VerifyRecursive(type, settings, path, GetNodeErrors)
                    .Finnish();
                return errors;
            }

            private static ErrorBuilder.TypeErrorsBuilder VerifyCore(MemberSettings settings, Type type)
            {
                return ErrorBuilder.Start()
                                   .CheckRequiresReferenceHandling(type, settings, t => !settings.IsImmutable(t))
                                   .CheckIsCopyableEnumerable(type, settings)
                                   .CheckIndexers(type, settings);
            }

            private static TypeErrors GetNodeErrors(MemberSettings settings, MemberPath path)
            {
                if (settings.ReferenceHandling == ReferenceHandling.References)
                {
                    return null;
                }

                var type = path.LastNodeType;
                return GetOrCreateErrors(type, settings, path);
            }
        }
    }
}
