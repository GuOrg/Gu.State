namespace Gu.ChangeTracking
{
    using System;
    using System.Reflection;

    public static partial class EqualBy
    {
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
            Verify.GetPropertiesErrors(type, settings).ThrowIfHasErrors(type, settings);
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
            Verify.GetFieldsErrors(type, settings)
                .ThrowIfHasErrors(type, settings);
        }

        internal static class Verify
        {
            internal static void CanEqualByPropertyValues<T>(T x, T y, PropertiesSettings settings)
            {
                var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
                GetPropertiesErrors(type, settings)
                    .ThrowIfHasErrors(type, settings);
            }

            internal static TypeErrors GetPropertiesErrors(Type type, PropertiesSettings settings, MemberPath path = null)
            {
                return settings.EqualByErrors.GetOrAdd(
                    type,
                    t => VerifyCore(settings, t)
                             .CheckProperties(t, settings, path, GetPropertyErrors));
            }

            internal static void CanEqualByFieldValues<T>(T x, T y, FieldsSettings settings)
            {
                var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
                GetFieldsErrors(type, settings)
                    .ThrowIfHasErrors(type, settings);
            }

            internal static TypeErrors GetFieldsErrors(Type type, FieldsSettings settings, MemberPath path = null)
            {
                return settings.EqualByErrors.GetOrAdd(
                    type,
                    t => VerifyCore(settings, t)
                             .CheckFields(t, settings, path, GetFieldErrors));
            }

            private static TypeErrors VerifyCore(IMemberSettings settings, Type type)
            {
                return ErrorBuilder.Start()
                                   .CheckReferenceHandlingIfEnumerable(type, settings)
                                   .CheckIndexers(type, settings);
            }

            private static TypeErrors GetPropertyErrors(Type type, PropertyInfo property, PropertiesSettings settings, MemberPath path)
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
                    return new TypeErrors(property.PropertyType);
                }

                return GetPropertiesErrors(property.PropertyType, settings, path);
            }

            private static TypeErrors GetFieldErrors(Type type, FieldInfo field, FieldsSettings settings, MemberPath path)
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
                    return new TypeErrors(field.FieldType);
                }

                return GetFieldsErrors(field.FieldType, settings, path);
            }
        }
    }
}
