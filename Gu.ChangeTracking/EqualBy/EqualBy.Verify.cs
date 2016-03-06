namespace Gu.ChangeTracking
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

        public static void VerifyCanEqualByPropertyValues(Type type, PropertiesSettings settings)
        {
            Verify.GetPropertiesErrors(type, settings)
                  .ThrowIfHasErrors(settings);
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

        public static void VerifyCanEqualByFieldValues(Type type, FieldsSettings settings)
        {
            Verify.GetFieldsErrors(type, settings)
                  .ThrowIfHasErrors(settings);
        }

        internal static class Verify
        {
            internal static void CanEqualByPropertyValues<T>(T x, T y, PropertiesSettings settings)
            {
                var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
                GetPropertiesErrors(type, settings)
                    .ThrowIfHasErrors(settings);
            }

            internal static TypeErrors GetPropertiesErrors(Type type, PropertiesSettings settings, MemberPath path = null)
            {
                return settings.EqualByErrors.GetOrAdd(
                    type,
                    t => VerifyCore(settings, t)
                             .VerifyRecursive(t, settings, path, GetRecursivePropertiesErrors));
            }

            internal static void CanEqualByFieldValues<T>(T x, T y, FieldsSettings settings)
            {
                var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
                GetFieldsErrors(type, settings)
                    .ThrowIfHasErrors(settings);
            }

            internal static TypeErrors GetFieldsErrors(Type type, FieldsSettings settings, MemberPath path = null)
            {
                return settings.EqualByErrors.GetOrAdd(
                    type,
                    t => VerifyCore(settings, t)
                             .VerifyRecursive(t, settings, path, GetRecursiveFieldsErrors));
            }

            private static TypeErrors VerifyCore(IMemberSettings settings, Type type)
            {
                return ErrorBuilder.Start()
                                   .CheckReferenceHandling(type, settings)
                                   .CheckIndexers(type, settings);
            }

            private static Error GetRecursivePropertiesErrors(Type type, PropertyInfo property, PropertiesSettings settings, MemberPath path)
            {
                if (property.PropertyType.IsEquatable())
                {
                    return null;
                }

                if (settings.ReferenceHandling == ReferenceHandling.References)
                {
                    return null;
                }

                if (settings.ReferenceHandling == ReferenceHandling.Throw)
                {
                    return new RequiresReferenceHandling(property.PropertyType);
                }

                return GetPropertiesErrors(property.PropertyType, settings, path);
            }

            private static Error GetRecursiveFieldsErrors(Type type, FieldInfo field, FieldsSettings settings, MemberPath path)
            {
                if (field.FieldType.IsEquatable())
                {
                    return null;
                }

                if (settings.ReferenceHandling == ReferenceHandling.References)
                {
                    return null;
                }

                if (settings.ReferenceHandling == ReferenceHandling.Throw)
                {
                    return new RequiresReferenceHandling(field.FieldType);
                }

                return GetFieldsErrors(field.FieldType, settings, path);
            }
        }
    }
}
