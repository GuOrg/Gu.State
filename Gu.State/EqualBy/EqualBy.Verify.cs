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
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used a deep equality check is performed.
        /// </param>
        public static void VerifyCanEqualByPropertyValues<T>(
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw)
        {
            var settings = PropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
            VerifyCanEqualByPropertyValues<T>(settings);
        }

        public static void VerifyCanEqualByPropertyValues<T>(PropertiesSettings settings)
        {
            var type = typeof(T);
            VerifyCanEqualByPropertyValues(type, settings);
        }

        public static void VerifyCanEqualByPropertyValues<T>(PropertiesSettings settings, string className, string methodName)
        {
            var type = typeof(T);
            VerifyCanEqualByPropertyValues(type, settings, className, methodName);
        }

        public static void VerifyCanEqualByPropertyValues(Type type, PropertiesSettings settings)
        {
            VerifyCanEqualByPropertyValues(type, settings, typeof(EqualBy).Name, nameof(EqualBy.PropertyValues));
        }

        public static void VerifyCanEqualByPropertyValues(Type type, PropertiesSettings settings, string className, string methodName)
        {
            Verify.GetPropertiesErrors(type, settings)
                  .ThrowIfHasErrors(settings, className, methodName);
        }

        public static void VerifyCanEqualByFieldValues<T>(
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw)
        {
            var settings = FieldsSettings.GetOrCreate(bindingFlags, referenceHandling);
            VerifyCanEqualByFieldValues<T>(settings);
        }

        public static void VerifyCanEqualByFieldValues<T>(FieldsSettings settings)
        {
            var type = typeof(T);
            VerifyCanEqualByFieldValues(type, settings);
        }

        public static void VerifyCanEqualByFieldValues<T>(FieldsSettings settings, string className, string methodName)
        {
            var type = typeof(T);
            VerifyCanEqualByFieldValues(type, settings, className, methodName);
        }

        public static void VerifyCanEqualByFieldValues(Type type, FieldsSettings settings)
        {
            VerifyCanEqualByFieldValues(type, settings, typeof(EqualBy).Name, nameof(EqualBy.FieldValues));
        }

        public static void VerifyCanEqualByFieldValues(Type type, FieldsSettings settings, string className, string methodName)
        {
            Verify.GetFieldsErrors(type, settings)
                  .ThrowIfHasErrors(settings, className, methodName);
        }

        internal static class Verify
        {
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
