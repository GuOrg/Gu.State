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

        internal static TypeErrors CheckReferenceHandling<TSetting>(this TypeErrors typeErrors, Type type, TSetting settings)
            where TSetting : IMemberSettings
        {
            if (typeof(IEnumerable).IsAssignableFrom(type) && settings.ReferenceHandling == ReferenceHandling.Throw)
            {
                typeErrors = typeErrors.CreateIfNull(type)
                                       .Add(new RequiresReferenceHandling(type));
            }

            return typeErrors;
        }

        internal static TypeErrors CheckIndexers<T>(this TypeErrors typeErrors, Type type, T settings)
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

                typeErrors = typeErrors.CreateIfNull(type)
                                       .Add(new UnsupportedIndexer(type, propertyInfo));
            }

            return typeErrors;
        }

        internal static TypeErrors VerifyRecursive(
            this TypeErrors typeErrors,
            Type type,
            PropertiesSettings settings,
            MemberPath memberPath,
            Func<Type, PropertyInfo, PropertiesSettings, MemberPath, Error> getPropertyErrors)
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

                typeErrors = VerifyRecursive(typeErrors, type, settings, memberPath, getPropertyErrors, propertyInfo);
            }

            return typeErrors;
        }

        internal static TypeErrors VerifyRecursive(
            this TypeErrors typeErrors,
            Type type,
            FieldsSettings settings,
            MemberPath memberPath,
            Func<Type, FieldInfo, FieldsSettings, MemberPath, Error> getFieldErrors)
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

                typeErrors = VerifyRecursive(typeErrors, type, settings, memberPath, getFieldErrors, fieldInfo);
            }

            return typeErrors;
        }

        private static TypeErrors VerifyRecursive<TMember, TSettings>(
            TypeErrors typeErrors,
            Type type,
            TSettings settings,
            MemberPath memberPath,
            Func<Type, TMember, TSettings, MemberPath, Error> getMemberErrors,
            TMember memberInfo)
            where TMember : MemberInfo
            where TSettings : class, IMemberSettings
        {
            if (memberPath.Contains(memberInfo))
            {
                if (settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops)
                {
                    return typeErrors;
                }

                if (settings.ReferenceHandling == ReferenceHandling.Structural)
                {
                    memberPath = memberPath.WithMember(memberInfo);
                    typeErrors = typeErrors.CreateIfNull(type)
                                           .Add(new ReferenceLoop(memberInfo, memberPath));
                    return typeErrors;
                }
            }

            memberPath = memberPath.WithMember(memberInfo);
            var memberErrors = getMemberErrors(type, memberInfo, settings, memberPath);
            if (memberErrors == null)
            {
                return typeErrors;
            }

            typeErrors = typeErrors.CreateIfNull(type)
                                   .Add(new MemberTypeErrors(memberInfo, memberPath, memberErrors));
            return typeErrors;
        }

        internal static TypeErrors CreateIfNull(this TypeErrors errors, Type type)
        {
            return errors ?? new TypeErrors(type);
        }
    }
}