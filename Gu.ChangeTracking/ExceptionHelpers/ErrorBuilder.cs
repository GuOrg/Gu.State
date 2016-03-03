namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Reflection;

    internal static class ErrorBuilder
    {
        public static Errors Start()
        {
            return null;
        }

        public static Errors HasReferenceHandlingIfEnumerable<TSetting>(this Errors errors, Type type, TSetting settings)
            where TSetting : IMemberSettings
        {
            if (typeof(IEnumerable).IsAssignableFrom(type) && settings.ReferenceHandling == ReferenceHandling.Throw)
            {
                errors = errors.Create(type);
                ((IErrors)errors).UnsupportedTypes.Add(type);
            }

            return errors;
        }

        public static Errors OnlySupportedIndexers<T>(this Errors errors, Type type, T settings)
            where T : IMemberSettings
        {
            var propertiesSettings = settings as PropertiesSettings;
            var propertyInfos = type.GetProperties(settings.BindingFlags);
            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.GetIndexParameters().Length == 0)
                {
                    continue;
                }

                if (propertiesSettings?.IsIgnoringProperty(propertyInfo) == true)
                {
                    continue;
                }

                if (settings.IsIgnoringDeclaringType(propertyInfo.DeclaringType))
                {
                    continue;
                }

                errors = errors.Create(type);
                ((IErrors)errors).UnsupportedIndexers.Add(propertyInfo);
            }

            return errors;
        }

        public static Errors OnlyValidProperties(this Errors errors, Type type, PropertiesSettings settings, Func<PropertyInfo, PropertiesSettings, bool> isPropertyValid)
        {
            var propertyInfos = type.GetProperties(settings.BindingFlags);
            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                if (settings.IsIgnoringProperty(propertyInfo))
                {
                    continue;
                }

                if (isPropertyValid(propertyInfo, settings))
                {
                    continue;
                }

                errors = errors.Create(type);
                ((IErrors)errors).UnsupportedProperties.Add(propertyInfo);
            }

            return errors;
        }

        public static Errors OnlyValidFields<T>(this Errors errors, Type type, T settings, Func<FieldInfo, T, bool> isPropertyValid)
            where T : IMemberSettings
        {
            var fields = type.GetFields(settings.BindingFlags);
            foreach (var field in fields)
            {
                if (field.IsEventField())
                {
                    continue;
                }

                if (isPropertyValid(field, settings))
                {
                    continue;
                }

                errors = errors.Create(type);
                ((IErrors)errors).UnsupportedFields.Add(field);
            }

            return errors;
        }

        private static Errors Create(this IErrors errors, Type type)
        {
            return (Errors)(errors ?? new Errors(type));
        }
    }
}