namespace Gu.ChangeTracking
{
    using System;
    using System.Reflection;

    public static partial class EqualBy
    {
        public static void VerifyCanEqualByPropertyValues<T>()
        {
            VerifyCanEqualByPropertyValues<T>(PropertiesSettings.GetOrCreate());
        }

        public static void VerifyCanEqualByPropertyValues<T>(PropertiesSettings settings)
        {
            var type = typeof(T);
            VerifyCanEqualByPropertyValues(type, settings);
        }

        public static void VerifyCanEqualByPropertyValues(Type type, PropertiesSettings settings)
        {
            VerifyCore(settings, type)
                .OnlyValidProperties(type, settings, IsPropertyValid)
                .ThrowIfHasErrors<PropertiesSettings>(type, settings);
        }

        public static void VerifyCanEqualByFieldValues<T>()
        {
            VerifyCanEqualByFieldValues<T>(FieldsSettings.GetOrCreate());
        }

        public static void VerifyCanEqualByFieldValues<T>(FieldsSettings settings)
        {
            var type = typeof(T);
            VerifyCore(settings, type)
                .OnlyValidFields(type, settings, IsFieldValid)
                .ThrowIfHasErrors<FieldsSettings>(type, settings);
        }

        private static Errors VerifyCore(IMemberSettings settings, Type type)
        {
            return ErrorBuilder.Start()
                               .HasReferenceHandlingIfEnumerable(type, settings)
                               .OnlySupportedIndexers(type, settings);
        }

        private static bool IsPropertyValid(PropertyInfo property, PropertiesSettings settings)
        {
            if (property.PropertyType.IsEquatable())
            {
                return true;
            }

            return settings.ReferenceHandling != ReferenceHandling.Throw;
        }

        private static bool IsFieldValid(FieldInfo field, FieldsSettings settings)
        {
            if (field.FieldType.IsEquatable())
            {
                return true;
            }

            return settings.ReferenceHandling != ReferenceHandling.Throw;
        }
    }
}
