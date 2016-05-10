namespace Gu.State
{
    using System;
    using System.Reflection;

    public static partial class EqualBy
    {
        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be compared for equality
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore properties for settings for</typeparam>
        /// <param name="referenceHandling">
        /// If Structural is used a deep equality check is performed.
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        public static void VerifyCanEqualByPropertyValues<T>(
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags)
        {
            var settings = PropertiesSettings.GetOrCreate(referenceHandling, bindingFlags);
            VerifyCanEqualByPropertyValues<T>(settings);
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be compared for equality
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <param name="settings">The settings to use.</param>
        public static void VerifyCanEqualByPropertyValues<T>(PropertiesSettings settings)
        {
            var type = typeof(T);
            VerifyCanEqualByPropertyValues(type, settings);
        }

        /// <summary>
        /// Check if the properties of <paramref name="type"/> can be compared for equality
        /// This method will throw an exception if copy cannot be performed for <paramref name="type"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="settings">The settings to use.</param>
        public static void VerifyCanEqualByPropertyValues(Type type, PropertiesSettings settings)
        {
            VerifyCanEqualByPropertyValues(type, settings, typeof(EqualBy).Name, nameof(EqualBy.PropertyValues));
        }

        /// <summary>
        /// Check if the fields of <typeparamref name="T"/> can be compared for equality
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for</typeparam>
        /// <param name="referenceHandling">
        /// If Structural is used a deep equality check is performed.
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting fields</param>
        public static void VerifyCanEqualByFieldValues<T>(
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags)
        {
            var settings = FieldsSettings.GetOrCreate(referenceHandling, bindingFlags);
            VerifyCanEqualByFieldValues<T>(settings);
        }

        /// <summary>
        /// Check if the fields of <typeparamref name="T"/> can be compared for equality
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <param name="settings">The settings to use.</param>
        public static void VerifyCanEqualByFieldValues<T>(FieldsSettings settings)
        {
            var type = typeof(T);
            VerifyCanEqualByFieldValues(type, settings);
        }

        /// <summary>
        /// Check if the fields of <paramref name="type"/> can be compared for equality
        /// This method will throw an exception if copy cannot be performed for <paramref name="type"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="settings">The settings to use.</param>
        public static void VerifyCanEqualByFieldValues(Type type, FieldsSettings settings)
        {
            VerifyCanEqualByFieldValues(type, settings, typeof(EqualBy).Name, nameof(EqualBy.FieldValues));
        }

        internal static void VerifyCanEqualByPropertyValues<T>(PropertiesSettings settings, string className, string methodName)
        {
            var type = typeof(T);
            VerifyCanEqualByPropertyValues(type, settings, className, methodName);
        }

        internal static void VerifyCanEqualByPropertyValues(Type type, PropertiesSettings settings, string className, string methodName)
        {
            Verify.GetPropertiesErrors(type, settings)
                  .ThrowIfHasErrors(settings, className, methodName);
        }

        internal static void VerifyCanEqualByFieldValues<T>(FieldsSettings settings, string className, string methodName)
        {
            var type = typeof(T);
            VerifyCanEqualByFieldValues(type, settings, className, methodName);
        }

        internal static void VerifyCanEqualByFieldValues(Type type, FieldsSettings settings, string className, string methodName)
        {
            Verify.GetFieldsErrors(type, settings)
                  .ThrowIfHasErrors(settings, className, methodName);
        }

        internal static class Verify
        {
            internal static void CanEqualByMemberValues<T>(T x, T y, IMemberSettings settings)
            {
                CanEqualByMemberValues(x, y, settings, typeof(EqualBy).Name, settings is PropertiesSettings ? nameof(EqualBy.PropertyValues) : nameof(EqualBy.FieldValues));
            }

            internal static void CanEqualByMemberValues<T>(T x, T y, IMemberSettings settings, string className, string methodName)
            {
                var propertiesSettings = settings as PropertiesSettings;
                if (propertiesSettings != null)
                {
                    CanEqualByPropertyValues(x, y, propertiesSettings, className, methodName);
                    return;
                }

                var fieldsSettings = settings as FieldsSettings;
                if (fieldsSettings != null)
                {
                    CanEqualByFieldValues(x, y, fieldsSettings, className, methodName);
                    return;
                }

                throw Throw.ExpectedParameterOfTypes<PropertiesSettings, FieldsSettings>("CanEqualByMemberValues failed.");
            }

            internal static void CanEqualByPropertyValues<T>(T x, T y, PropertiesSettings settings)
            {
                CanEqualByPropertyValues(x, y, settings, typeof(EqualBy).Name, nameof(EqualBy.PropertyValues));
            }

            internal static void CanEqualByPropertyValues<T>(T x, T y, PropertiesSettings settings, string className, string methodName)
            {
                var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
                GetPropertiesErrors(type, settings)
                    .ThrowIfHasErrors(settings, className, methodName);
            }

            internal static TypeErrors GetPropertiesErrors(Type type, PropertiesSettings settings, MemberPath path = null)
            {
                return settings.EqualByErrors.GetOrAdd(
                    type,
                    t => VerifyCore(settings, t)
                             .VerifyRecursive(t, settings, path, GetRecursivePropertiesErrors)
                             .Finnish());
            }

            internal static void CanEqualByFieldValues<T>(T x, T y, FieldsSettings settings)
            {
                CanEqualByFieldValues(x, y, settings, typeof(EqualBy).Name, nameof(EqualBy.FieldValues));
            }

            internal static void CanEqualByFieldValues<T>(T x, T y, FieldsSettings settings, string className, string methodName)
            {
                var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
                GetFieldsErrors(type, settings)
                    .ThrowIfHasErrors(settings, className, methodName);
            }

            internal static TypeErrors GetFieldsErrors(Type type, FieldsSettings settings, MemberPath path = null)
            {
                return settings.EqualByErrors.GetOrAdd(
                    type,
                    t => VerifyCore(settings, t)
                             .VerifyRecursive(t, settings, path, GetRecursiveFieldsErrors)
                             .Finnish());
            }

            private static ErrorBuilder.TypeErrorsBuilder VerifyCore(IMemberSettings settings, Type type)
            {
                return ErrorBuilder.Start()
                                   .CheckRequiresReferenceHandling(type, settings, t => !settings.IsEquatable(t))
                                   .CheckIndexers(type, settings);
            }

            private static TypeErrors GetRecursivePropertiesErrors(PropertiesSettings settings, MemberPath path)
            {
                var type = path.LastNodeType;
                if (settings.IsEquatable(type))
                {
                    return null;
                }

                if (settings.ReferenceHandling == ReferenceHandling.References)
                {
                    return null;
                }

                return GetPropertiesErrors(type, settings, path);
            }

            private static TypeErrors GetRecursiveFieldsErrors(FieldsSettings settings, MemberPath path)
            {
                var type = path.LastNodeType;
                if (settings.IsEquatable(type))
                {
                    return null;
                }

                if (settings.ReferenceHandling == ReferenceHandling.References)
                {
                    return null;
                }

                return GetFieldsErrors(type, settings, path);
            }
        }
    }
}
