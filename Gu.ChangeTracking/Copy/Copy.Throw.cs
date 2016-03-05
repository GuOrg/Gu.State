namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    public static partial class Copy
    {
        private static StringBuilder AppendCopyFailed<T>(this StringBuilder errorBuilder)
            where T : IMemberSettings
        {
            if (typeof(FieldsSettings).IsAssignableFrom(typeof(T)))
            {
                return errorBuilder.CreateIfNull()
                                   .AppendLine($"Copy.{nameof(Copy.FieldValues)}(x, y) failed.");
            }

            if (typeof(PropertiesSettings).IsAssignableFrom(typeof(T)))
            {
                return errorBuilder.CreateIfNull()
                                   .AppendLine($"Copy.{nameof(Copy.PropertyValues)}(x, y) failed.");
            }

            throw ChangeTracking.Throw.ExpectedParameterOfTypes<FieldsSettings, PropertiesSettings>("{T}");
        }

        // ReSharper disable once UnusedParameter.Local
        private static void ThrowIfHasErrors<TSetting>(this TypeErrors errors, TSetting settings)
            where TSetting : class, IMemberSettings
        {
            if (errors == null)
            {
                return;
            }

            var errorBuilder = new StringBuilder();
            errorBuilder.AppendCopyFailed<TSetting>()
                        .AppendNotSupported(errors)
                        .AppendSolveTheProblemBy()
                        .AppendSuggestImmutable(errors)
                        .AppendLine($"* Use {typeof(TSetting).Name} and specify how copying is performed:")
                        .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Structural)} means that a the entire graph is traversed and immutable property values are copied..")
                        .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.StructuralWithReferenceLoops)} means that a the entire graph is traversed and immutable property values are copied and reference loops are tracked.")
                        .AppendLine("For structural Activator.CreateInstance is used so a parameterless constructor must be present, can be private.")
                        .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.References)} means that references are copied.")
                        .AppendSuggestExclude(errors);

            var message = errorBuilder.ToString();
            throw new NotSupportedException(message);
        }

        private static StringBuilder AppendSuggestCopySettings<T>(this StringBuilder errorBuilder, Type type, MemberInfo member)
            where T : IMemberSettings
        {
            errorBuilder = errorBuilder.CreateIfNull()
                                       .AppendLine($"* Use {typeof(T).Name} and specify how copying is performed:")
                                       .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Structural)} means that a deep copy is performed.")
                                       .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.References)} means that references are copied.")
                                       .AppendExcludeType(type);
            if (member != null)
            {
                if (typeof(T) == typeof(FieldsSettings))
                {
                    errorBuilder.AppendExcludeField(type, member as FieldInfo);
                    var indexer = member as PropertyInfo;

                    if (indexer != null)
                    {
                        Debug.Assert(indexer.GetIndexParameters().Length > 0, "Must be an indexer");
                    }
                }
                else if (typeof(T) == typeof(PropertiesSettings))
                {
                    errorBuilder.AppendExcludeProperty(type, member as PropertyInfo);
                }
                else
                {
                    throw ChangeTracking.Throw.ExpectedParameterOfTypes<FieldsSettings, PropertiesSettings>("{T}");
                }
            }

            return errorBuilder;
        }

        // ReSharper disable once UnusedParameter.Local
        private static StringBuilder AppendCannotCopyMember<T>(this StringBuilder errorBuilder, Type sourceType, MemberInfo member, T settings)
            where T : IMemberSettings
        {
            return errorBuilder.AppendCannotCopyMember<T>(sourceType, member);
        }

        private static StringBuilder AppendCannotCopyMember<T>(this StringBuilder errorBuilder, Type sourceType, MemberInfo member)
            where T : IMemberSettings
        {
            return errorBuilder.CreateIfNull()
                               .AppendCopyFailed<T>()
                               .AppendMemberIsNotSupported(sourceType, member)
                               .AppendSolveTheProblemBy()
                               .AppendSuggestImmutableType(member.MemberType())
                               .AppendSuggestCopySettings<T>(sourceType, member);
        }

        private static StringBuilder AppendCannotCopyIndexer<T>(
            this StringBuilder errorBuilder,
            Type sourceType,
            PropertyInfo indexer)
            where T : IMemberSettings
        {
            Debug.Assert(indexer.GetIndexParameters().Length > 0, "Must be an indexer");
            return errorBuilder.CreateIfNull()
                           .AppendCopyFailed<T>()
                           .AppendPropertyIsNotSupported(sourceType, indexer)
                           .AppendSolveTheProblemBy()
                           .AppendSuggestCopySettings<T>(sourceType, indexer);
        }

        // ReSharper disable once UnusedParameter.Local
        private static StringBuilder AppendCannotCopyType<T>(this StringBuilder errorBuilder, Type type, T settings)
            where T : IMemberSettings
        {
            return errorBuilder.CreateIfNull()
                               .AppendCopyFailed<T>()
                               .AppendTypeIsNotSupported(type)
                               .AppendSolveTheProblemBy()
                               .AppendSuggestImmutableType(type)
                               .AppendSuggestCopySettings<T>(type, null);
        }

        private static class Throw
        {
            // ReSharper disable once UnusedParameter.Local
            internal static void CannotCopyMember<T>(Type sourceType, MemberInfo member, T settings)
                where T : IMemberSettings
            {
                CannotCopyMember<T>(sourceType, member);
            }

            internal static void CannotCopyMember<T>(Type sourceType, MemberInfo member)
                where T : IMemberSettings
            {
                var errorBuilder = new StringBuilder().AppendCannotCopyMember<T>(sourceType, member);
                var message = errorBuilder.ToString();
                throw new NotSupportedException(message);
            }

            // ReSharper disable once UnusedParameter.Local
            internal static void ReadonlyMemberDiffers<T>(
                SourceAndTargetValue sourceAndTargetValue,
                MemberInfo member,
                T settings)
                where T : IMemberSettings
            {
                var errorBuilder = new StringBuilder();
                errorBuilder.AppendCopyFailed<T>();
                var propertyInfo = member as PropertyInfo;
                if (propertyInfo != null)
                {
                    errorBuilder.AppendLine($"The readonly property {sourceAndTargetValue.Source.GetType().PrettyName()}.{member.Name} differs after copy.");
                    errorBuilder.AppendLine($" - Source value: {sourceAndTargetValue.SourceValue}.");
                    errorBuilder.AppendLine($" - Target value: {sourceAndTargetValue.TargeteValue}.");
                    errorBuilder.AppendLine($"The property is of type {propertyInfo.PropertyType.PrettyName()}.");
                }
                else
                {
                    var fieldInfo = member as FieldInfo;
                    if (fieldInfo != null)
                    {
                        errorBuilder.AppendLine($"The readonly field {sourceAndTargetValue.Source.GetType().PrettyName()}.{member.Name} differs after copy.");
                        errorBuilder.AppendLine($" - Source value: {sourceAndTargetValue.SourceValue}.");
                        errorBuilder.AppendLine($" - Target value: {sourceAndTargetValue.TargeteValue}.");
                        errorBuilder.AppendLine($"The field is of type {fieldInfo.FieldType.PrettyName()}.");
                    }
                    else
                    {
                        throw ChangeTracking.Throw.ExpectedParameterOfTypes<PropertyInfo, FieldInfo>(nameof(member));
                    }
                }

                errorBuilder.AppendSolveTheProblemBy()
                    .AppendSuggestCopySettings<T>(member.DeclaringType, member);
                throw new InvalidOperationException(errorBuilder.ToString());
            }

            internal static void CannotCopyType<T>(Type type, T settings)
                where T : IMemberSettings
            {
                var errorBuilder = new StringBuilder();
                AppendCannotCopyType(errorBuilder, type, settings);
                throw new NotSupportedException(errorBuilder.ToString());
            }

            internal static void CannotCopyItem<T>(IList source, IList target, int index, T settings)
                where T : IMemberSettings
            {
                var itemType = source[index]?.GetType() ?? source.GetType().GetItemType();
                var errorBuilder = new StringBuilder();
                if (settings is PropertiesSettings)
                {
                    errorBuilder.AppendLine(
                        $"Copy.{nameof(PropertyValues)}(x, y) does not support copying the type {itemType.PrettyName()}");
                }
                else if (settings is FieldsSettings)
                {
                    errorBuilder.AppendLine($"Copy.{nameof(CopyFieldsValues)}(x, y) does not support copying the type {itemType.PrettyName()}");
                }
                else
                {
                    throw ChangeTracking.Throw.ExpectedParameterOfTypes<PropertiesSettings, FieldsSettings>(nameof(settings));
                }

                errorBuilder.AppendLine($"The problem occurred at index: {index}")
                    .AppendLine(
                        $"Source list is of type: {source.GetType().PrettyName()} and target list is of type: {target.GetType().PrettyName()}")
                    .AppendSolveTheProblemBy()
                    .AppendSuggestImmutableType(itemType)
                    .AppendSuggestCopySettings<T>(itemType, null);
                throw new NotSupportedException(errorBuilder.ToString());
            }

            internal static void CannotCopyItem<T>(IDictionary source, IDictionary target, object key, T settings)
                where T : IMemberSettings
            {
                var itemType = source[key]?.GetType();
                var errorBuilder = new StringBuilder();
                if (settings is PropertiesSettings)
                {
                    errorBuilder.AppendLine($"Copy.{nameof(PropertyValues)}(x, y) does not support copying the type {itemType.PrettyName()}");
                }
                else if (settings is FieldsSettings)
                {
                    errorBuilder.AppendLine(
                        $"Copy.{nameof(CopyFieldsValues)}(x, y) does not support copying the type {itemType.PrettyName()}");
                }
                else
                {
                    throw ChangeTracking.Throw.ExpectedParameterOfTypes<PropertiesSettings, FieldsSettings>(nameof(settings));
                }

                errorBuilder.AppendLine($"The problem occurred for key: {key}")
                    .AppendLine($"Source list is of type: {source.GetType().PrettyName()} and target list is of type: {target.GetType().PrettyName()}")
                    .AppendSolveTheProblemBy()
                    .AppendSuggestImmutableType(itemType)
                    .AppendSuggestCopySettings<T>(itemType, null);
                throw new NotSupportedException(errorBuilder.ToString());
            }

            // ReSharper disable once UnusedParameter.Local
            internal static void CannotCopyFixesSizeCollections<T>(ICollection source, ICollection target, T settings)
                where T : IMemberSettings
            {
                var errorBuilder = new StringBuilder();
                errorBuilder.AppendCopyFailed<T>()
                            .AppendLine($"The collections are fixed size type: {source.GetType().PrettyName()}")
                            .AppendLine($"Source count: {source.Count}")
                            .AppendLine($"Target count: {target.Count}")
                            .AppendSolveTheProblemBy()
                            .AppendLine("* Use a resizable collection like List<T>.")
                            .AppendLine("* Check that the collections are the same size before calling.");
                throw new InvalidOperationException(errorBuilder.ToString());
            }
        }
    }
}
