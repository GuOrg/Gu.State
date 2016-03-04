namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Reflection;

    internal static class ErrorBuilder
    {
        public static TypeErrors Start()
        {
            return null;
        }

        public static TypeErrors CheckReferenceHandlingIfEnumerable<TSetting>(this TypeErrors typeErrors, Type type, TSetting settings)
            where TSetting : IMemberSettings
        {
            if (typeof(IEnumerable).IsAssignableFrom(type) && settings.ReferenceHandling == ReferenceHandling.Throw)
            {
                typeErrors = typeErrors.CreateIfNull(type);
                typeErrors.Errors.Add(new RequiresReferenceHandling(type));
            }

            return typeErrors;
        }

        public static TypeErrors CheckIndexers<T>(this TypeErrors typeErrors, Type type, T settings)
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

                typeErrors = typeErrors.CreateIfNull(type);
                typeErrors.Errors.Add(new UnsupportedIndexer(type, propertyInfo));
            }

            return typeErrors;
        }

        public static TypeErrors CheckProperties(
            this TypeErrors typeErrors,
            Type type,
            PropertiesSettings settings,
            MemberPath memberPath,
            Func<Type, PropertyInfo, PropertiesSettings, MemberPath, TypeErrors> getPropertyErrors)
        {
            if (settings.ReferenceHandling == ReferenceHandling.References)
            {
                return typeErrors;
            }

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

                if (memberPath == null)
                {
                    memberPath = new MemberPath(type);
                }

                CheckMember(ref typeErrors, type, settings, memberPath, getPropertyErrors, propertyInfo);
            }

            return typeErrors;
        }

        public static TypeErrors CheckFields(
            this TypeErrors typeErrors,
            Type type,
            FieldsSettings settings,
            MemberPath memberPath,
            Func<Type, FieldInfo, FieldsSettings, MemberPath, TypeErrors> getFieldErrors)
        {
            if (settings.ReferenceHandling == ReferenceHandling.References)
            {
                return typeErrors;
            }

            var fields = type.GetFields(settings.BindingFlags);
            foreach (var fieldInfo in fields)
            {
                if (settings.IsIgnoringField(fieldInfo))
                {
                    continue;
                }

                if (memberPath == null)
                {
                    memberPath = new MemberPath(type);
                }

                CheckMember(ref typeErrors, type, settings, memberPath, getFieldErrors, fieldInfo);
            }

            return typeErrors;
        }

        private static void CheckMember<TMember, TSettings>(
            ref TypeErrors typeErrors,
            Type type,
            TSettings settings,
            MemberPath memberPath,
            Func<Type, TMember, TSettings, MemberPath, TypeErrors> getMemberErrors,
            TMember memberInfo)
            where TMember : MemberInfo
            where TSettings : class, IMemberSettings
        {
            if (memberPath.Contains(memberInfo))
            {
                if (settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops)
                {
                    return;
                }

                if (settings.ReferenceHandling == ReferenceHandling.Structural)
                {
                    typeErrors = typeErrors.CreateIfNull(type);
                    memberPath = memberPath.WithMember(memberInfo);
                    typeErrors.Errors.Add(new ReferenceLoop(memberInfo, memberPath));
                    return;
                }
            }

            memberPath = memberPath.WithMember(memberInfo);
            var propertyErrors = getMemberErrors(type, memberInfo, settings, memberPath);
            if (propertyErrors == null)
            {
                return;
            }

            typeErrors = typeErrors.CreateIfNull(type);
            typeErrors.Errors.Add(propertyErrors);
        }

        private static TypeErrors CreateIfNull(this TypeErrors errors, Type type)
        {
            return errors ?? new TypeErrors(type);
        }
    }
}