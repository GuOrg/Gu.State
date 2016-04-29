namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reflection;

    internal static partial class ErrorBuilder
    {
        internal static TypeErrorsBuilder Start()
        {
            return null;
        }

        internal static TypeErrors Finnish(this TypeErrorsBuilder builder)
        {
            if (builder == null || builder.Errors.Count == 0)
            {
                return null;
            }

            return new TypeErrors(builder);
        }

        internal static TypeErrors Merge(this TypeErrors first, TypeErrors other)
        {
            if (first == null)
            {
                return other;
            }

            if (other == null)
            {
                return first;
            }

            if (first == other)
            {
                return first;
            }

            if (first.Type == other.Type)
            {
                if (first.Errors.Count == 0)
                {
                    return other;
                }

                if (other.Errors.Count == 0)
                {
                    return first;
                }

                var errors = new MergedErrors(first.Errors, other.Errors);
                return new TypeErrors(first.Type, errors);
            }

            return new TypeErrors(null, new[] { first, other });
        }

        internal static TypeErrorsBuilder CheckRequiresReferenceHandling<TSettings>(
            this TypeErrorsBuilder typeErrors,
            Type type,
            TSettings settings,
            Func<Type, bool> requiresReferenceHandling)
            where TSettings : IMemberSettings
        {
            if (settings.ReferenceHandling == ReferenceHandling.Throw)
            {
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    if (type.Implements(typeof(IDictionary<,>)))
                    {
                        var arguments = type.GetGenericArguments();
                        if (arguments.Length != 2 ||
                            requiresReferenceHandling(arguments[0]) ||
                            requiresReferenceHandling(arguments[1]))
                        {
                            typeErrors = typeErrors.CreateIfNull(type)
                                                   .Add(RequiresReferenceHandling.Enumerable);
                        }
                    }
                    else if (requiresReferenceHandling(type.GetItemType()))
                    {
                        typeErrors = typeErrors.CreateIfNull(type)
                                               .Add(RequiresReferenceHandling.Enumerable);
                    }
                }
                else if (type.IsKeyValuePair())
                {
                    var arguments = type.GetGenericArguments();
                    if (requiresReferenceHandling(arguments[0]) || requiresReferenceHandling(arguments[1]))
                    {
                        typeErrors = typeErrors.CreateIfNull(type)
                                               .Add(RequiresReferenceHandling.ComplexType);
                    }
                }
                else if (requiresReferenceHandling(type))
                {
                    typeErrors = typeErrors.CreateIfNull(type)
                                           .Add(RequiresReferenceHandling.ComplexType);
                }
            }

            return typeErrors;
        }

        internal static TypeErrorsBuilder CheckNotifies<TSettings>(this TypeErrorsBuilder typeErrors, Type type, TSettings settings)
             where TSettings : IMemberSettings
        {
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                if (!typeof(INotifyCollectionChanged).IsAssignableFrom(type))
                {
                    return typeErrors.CreateIfNull(type)
                                           .Add(CollectionMustNotifyError.GetOrCreate(type));
                }
            }
            else if (!typeof(INotifyPropertyChanged).IsAssignableFrom(type))
            {
                if (settings.IsIgnoringDeclaringType(type))
                {
                    return typeErrors;
                }

                return typeErrors.CreateIfNull(type)
                                 .Add(TypeMustNotifyError.GetOrCreate(type));
            }

            return typeErrors;
        }

        internal static TypeErrorsBuilder CheckIndexers<TSettings>(this TypeErrorsBuilder typeErrors, Type type, TSettings settings)
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
                                       .Add(UnsupportedIndexer.GetOrCreate(propertyInfo));
            }

            return typeErrors;
        }

        internal static TypeErrorsBuilder VerifyRecursive(
            this TypeErrorsBuilder typeErrors,
            Type type,
            PropertiesSettings settings,
            MemberPath memberPath,
            Func<PropertiesSettings, MemberPath, TypeErrors> getPropertyErrors)
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

        internal static TypeErrorsBuilder VerifyRecursive(
            this TypeErrorsBuilder typeErrors,
            Type type,
            FieldsSettings settings,
            MemberPath memberPath,
            Func<FieldsSettings, MemberPath, TypeErrors> getFieldErrors)
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

        internal static TypeErrorsBuilder CreateIfNull(this TypeErrorsBuilder errors, Type type)
        {
            return errors ?? new TypeErrorsBuilder(type);
        }

        private static TypeErrorsBuilder VerifyEnumerableRecursively<TSettings>(
            TypeErrorsBuilder typeErrors,
            Type type,
            TSettings settings,
            MemberPath memberPath,
            Func<TSettings, MemberPath, TypeErrors> getErrorsRecursively)
            where TSettings : class, IMemberSettings
        {
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                memberPath = memberPath == null
                                 ? new MemberPath(type)
                                 : memberPath.WithCollectionItem(type);

                var recursiveErrors = getErrorsRecursively(settings, memberPath);
                if (recursiveErrors == null)
                {
                    return typeErrors;
                }

                var collectionErrors = new CollectionErrors(memberPath, recursiveErrors);
                typeErrors = typeErrors.CreateIfNull(type)
                                       .Add(collectionErrors);
            }

            return typeErrors;
        }

        private static TypeErrorsBuilder VerifyMemberRecursively<TSettings>(
            TypeErrorsBuilder typeErrors,
            Type type,
            TSettings settings,
            MemberPath memberPath,
            Func<TSettings, MemberPath, TypeErrors> getErrorsRecursively,
            MemberInfo memberInfo)
            where TSettings : class, IMemberSettings
        {
            memberPath = memberPath.WithMember(memberInfo);
            if (memberPath.HasLoop())
            {
                if (settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops)
                {
                    return typeErrors;
                }

                if (settings.ReferenceHandling == ReferenceHandling.Structural)
                {
                    typeErrors = typeErrors.CreateIfNull(type)
                                           .Add(new ReferenceLoop(memberPath));
                    return typeErrors;
                }
            }

            var recursiveErrors = getErrorsRecursively(settings, memberPath);
            if (recursiveErrors == null)
            {
                return typeErrors;
            }

            var memberErrors = new MemberErrors(memberPath, recursiveErrors);
            typeErrors = typeErrors.CreateIfNull(type)
                                   .Add(memberErrors);
            return typeErrors;
        }
    }
}