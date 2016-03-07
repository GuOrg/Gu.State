namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
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

        internal static TypeErrors CheckNotifies<TSettings>(this TypeErrors typeErrors, Type type, TSettings settings)
             where TSettings : IMemberSettings
        {
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                if (!typeof(INotifyCollectionChanged).IsAssignableFrom(type))
                {
                    return typeErrors.CreateIfNull(type)
                                           .Add(new CollectionMustNotifyError(type));
                }
            }
            else if (!typeof(INotifyPropertyChanged).IsAssignableFrom(type))
            {
                if (settings.IsIgnoringDeclaringType(type))
                {
                    return typeErrors;
                }

                return typeErrors.CreateIfNull(type)
                                 .Add(new TypeMustNotifyError(type));
            }

            return typeErrors;
        }

        internal static TypeErrors CheckIndexers<TSettings>(this TypeErrors typeErrors, Type type, TSettings settings)
            where TSettings : IMemberSettings
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
            Func<PropertiesSettings, MemberPath, Error> getPropertyErrors)
        {
            if (settings.ReferenceHandling == ReferenceHandling.References)
            {
                return typeErrors;
            }

            typeErrors = VerifyEnumerableRecursively(typeErrors, type, settings, memberPath, getPropertyErrors);

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

                typeErrors = VerifyMemberRecursively(typeErrors, type, settings, memberPath, getPropertyErrors, propertyInfo);
            }

            return typeErrors;
        }

        internal static TypeErrors VerifyRecursive(
            this TypeErrors typeErrors,
            Type type,
            FieldsSettings settings,
            MemberPath memberPath,
            Func<FieldsSettings, MemberPath, Error> getFieldErrors)
        {
            if (settings.ReferenceHandling == ReferenceHandling.References)
            {
                return typeErrors;
            }

            typeErrors = VerifyEnumerableRecursively(typeErrors, type, settings, memberPath, getFieldErrors);

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

                typeErrors = VerifyMemberRecursively(typeErrors, type, settings, memberPath, getFieldErrors, fieldInfo);
            }

            return typeErrors;
        }

        private static TypeErrors VerifyEnumerableRecursively<TSettings>(
            TypeErrors typeErrors,
            Type type,
            TSettings settings,
            MemberPath memberPath,
            Func<TSettings, MemberPath, Error> getErrorsRecursively)
            where TSettings : class, IMemberSettings
        {
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                memberPath = memberPath == null
                                 ? new MemberPath(type)
                                 : memberPath.WithCollectionItem(type);

                var memberErrors = getErrorsRecursively(settings, memberPath);
                if (memberErrors == null)
                {
                    return typeErrors;
                }

                typeErrors = typeErrors.CreateIfNull(type)
                                       .Add(new CollectionError(memberPath, memberErrors));
            }

            return typeErrors;
        }

        private static TypeErrors VerifyMemberRecursively<TSettings>(
            TypeErrors typeErrors,
            Type type,
            TSettings settings,
            MemberPath memberPath,
            Func<TSettings, MemberPath, Error> getErrorsRecursively,
            MemberInfo memberInfo)
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
            var memberErrors = getErrorsRecursively(settings, memberPath);
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