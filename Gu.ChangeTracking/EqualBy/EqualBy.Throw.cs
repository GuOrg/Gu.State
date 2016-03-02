namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Text;

    public static partial class EqualBy
    {
        private static StringBuilder AppendSuggestEqualBySettings<T>(this StringBuilder messageBuilder, Type type, MemberInfo member)
            where T : class, IMemberSettings
        {
            return messageBuilder.CreateIfNull()
                                 .AppendLine($"* Use {typeof(T).Name} and specify how comparing is performed:")
                                 .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Structural)} means that a deep equals is performed.")
                                 .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.References)} means that reference equality is used.")
                                 .AppendExcludeType(type)
                                 .AppendExcludeMember(type, member);
        }

        private static StringBuilder AppendEqualByFailed<T>(this StringBuilder errorBuilder)
            where T : class, IMemberSettings
        {
            if (typeof(IIgnoringProperties).IsAssignableFrom(typeof(T)))
            {
                errorBuilder.AppendLine($"EqualBy.{nameof(PropertyValues)}(x, y) failed.");
            }
            else if (typeof(IIgnoringFields).IsAssignableFrom(typeof(T)))
            {
                errorBuilder.AppendLine($"EqualBy.{nameof(FieldValues)}(x, y) failed.");
            }
            else
            {
                Gu.ChangeTracking.Throw.ThrowThereIsABugInTheLibraryExpectedParameterOfTypes<PropertiesSettings, FieldsSettings>("T");
            }

            return errorBuilder;
        }

        private static StringBuilder AppendCannotEquateIndexer<T>(
           this StringBuilder errorBuilder,
            Type sourceType,
            PropertyInfo indexer)
            where T : class, IMemberSettings
        {
            var member = typeof(PropertiesSettings).IsAssignableFrom(typeof(T))
                             ? indexer
                             : null;
            return errorBuilder.CreateIfNull()
                               .AppendEqualByFailed<T>()
                               .AppendPropertyIsNotSupported(sourceType, indexer)
                               .AppendSolveTheProblemBy()
                               .AppendSuggestEqualBySettings<T>(sourceType, member);
        }

        private static class Throw
        {
            internal static void CannotCompareMember(Type sourceType, MemberInfo member)
            {
                var errorBuilder = new StringBuilder();
                Type memberType = null;
                var propertyInfo = member as PropertyInfo;
                if (propertyInfo != null)
                {
                    errorBuilder.AppendEqualByFailed<PropertiesSettings>();
                    errorBuilder.AppendLine(
                        $"The property {sourceType.PrettyName()}.{propertyInfo.Name} is not supported.");
                    errorBuilder.AppendLine($"The property is of type {propertyInfo.PropertyType.PrettyName()}.");
                    memberType = propertyInfo.PropertyType;
                }
                else
                {
                    var fieldInfo = member as FieldInfo;
                    if (fieldInfo != null)
                    {
                        errorBuilder.AppendEqualByFailed<FieldsSettings>();
                        errorBuilder.AppendLine(
                            $"The field {sourceType.PrettyName()}.{fieldInfo.Name} is not supported.");
                        errorBuilder.AppendLine($"The field is of type {fieldInfo.FieldType.PrettyName()}.");
                        memberType = fieldInfo.FieldType;
                    }
                    else
                    {
                        Gu.ChangeTracking.Throw
                          .ThrowThereIsABugInTheLibraryExpectedParameterOfTypes<PropertyInfo, FieldInfo>(nameof(member));
                    }
                }

                errorBuilder.AppendSolveTheProblemBy()
                            .AppendSuggestImplementIEquatable(memberType)
                            .AppendSuggestEqualBySettings<FieldsSettings>(sourceType, member);
                var message = errorBuilder.ToString();
                throw new NotSupportedException(message);
            }

            // ReSharper disable once UnusedParameter.Local
            internal static void CannotCompareType<T>(Type type, T settings)
                where T : class, IMemberSettings
            {
                CannotCompareType<T>(type);
            }

            internal static void CannotCompareType<T>(Type type)
                where T : class, IMemberSettings
            {
                var errorBuilder = new StringBuilder();
                errorBuilder.AppendEqualByFailed<T>()
                            .AppendSolveTheProblemBy()
                            .AppendSuggestImplementIEquatable(type)
                            .AppendSuggestEqualBySettings<PropertiesSettings>(type, null);
                throw new NotSupportedException(errorBuilder.ToString());
            }
        }

        private static class Verify
        {
            public static void Indexers<T>(Type type, T settings)
                where T : class, IMemberSettings
            {
                if (type == null)
                {
                    return;
                }

                var errorBuilder = Indexers(type, settings, null);
                if (errorBuilder != null)
                {
                    var message = errorBuilder.ToString();
                    throw new NotSupportedException(message);
                }
            }

            public static StringBuilder Indexers<T>(Type type, T settings, StringBuilder errorBuilder)
                where T : class, IMemberSettings
            {
                var ignoringDeclaredType = settings as IIgnoringDeclaredType;
                var propertyInfos = type.GetProperties(Constants.DefaultFieldBindingFlags);
                foreach (var propertyInfo in propertyInfos)
                {
                    if (propertyInfo.GetIndexParameters()
                                    .Length == 0)
                    {
                        continue;
                    }

                    if (ignoringDeclaredType?.IsIgnoringDeclaringType(propertyInfo.DeclaringType) == true)
                    {
                        continue;
                    }

                    if (errorBuilder == null)
                    {
                        errorBuilder = new StringBuilder();
                    }

                    errorBuilder = errorBuilder.AppendCannotEquateIndexer<T>(type, propertyInfo);
                }

                return errorBuilder;
            }

            public static void Enumerable<T, TSetting>(T x, T y, TSetting settings)
                 where TSetting : class, IMemberSettings
            {
                if (settings.ReferenceHandling != ReferenceHandling.Throw)
                {
                    return;
                }

                var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    Throw.CannotCompareType(type, settings);
                }
            }

            public static void PropertyTypes<T>(T x, T y, PropertiesSettings settings)
            {
                if (settings.ReferenceHandling != ReferenceHandling.Throw)
                {
                    return;
                }

                var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
                var properties = type.GetProperties(settings.BindingFlags);
                foreach (var propertyInfo in properties)
                {
                    if (settings.IsIgnoringProperty(propertyInfo))
                    {
                        continue;
                    }

                    if (!propertyInfo.PropertyType.IsEquatable())
                    {
                        Throw.CannotCompareMember(type, propertyInfo);
                    }
                }
            }

            public static void FieldTypes<T>(T x, T y, FieldsSettings settings)
            {
                if (settings.ReferenceHandling != ReferenceHandling.Throw)
                {
                    return;
                }

                var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
                var fields = type.GetFields(settings.BindingFlags);
                foreach (var fieldInfo in fields)
                {
                    if (settings.IsIgnoringField(fieldInfo))
                    {
                        continue;
                    }

                    if (!fieldInfo.FieldType.IsEquatable())
                    {
                        Throw.CannotCompareMember(type, fieldInfo);
                    }
                }
            }
        }
    }
}
