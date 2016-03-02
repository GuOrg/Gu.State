namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    public static partial class Copy
    {
        private static StringBuilder AppendCopyFailed<T>(this StringBuilder messageBuilder)
            where T : CopySettings
        {
            var line = typeof(T) == typeof(CopyFieldsSettings)
                ? $"Copy.{nameof(Copy.FieldValues)}(x, y) failed."
                : $"Copy.{nameof(Copy.PropertyValues)}(x, y) failed.";
            return messageBuilder.AppendLine(line);
        }

        private static StringBuilder AppendSuggestCopySettings<T>(this StringBuilder errorBuilder, Type type, MemberInfo member)
            where T : CopySettings
        {
            errorBuilder.AppendLine($"* Use {typeof(T).Name} and specify how copying is performed:");
            errorBuilder.AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Structural)} means that a deep copy is performed.");
            errorBuilder.AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.References)} means that references are copied.");
            errorBuilder.AppendLine($"  - Exclude the type {type.PrettyName()}.");
            if (member != null)
            {
                if (typeof(T) == typeof(CopyFieldsSettings))
                {
                    var indexer = member as PropertyInfo;
                    if (indexer == null)
                    {
                        errorBuilder.AppendLine($"  - Exclude the field {type.PrettyName()}.{member.Name}.");
                    }
                    else
                    {
                        Debug.Assert(indexer.GetIndexParameters().Length > 0, "Must be an indexer");
                    }
                }
                else if (typeof(T) == typeof(CopyPropertiesSettings))
                {
                    errorBuilder.AppendLine($"  - Exclude the property {type.PrettyName()}.{member.Name}.");
                }
                else
                {
                    Gu.ChangeTracking.Throw.ThrowThereIsABugInTheLibraryExpectedParameterOfTypes<CopyFieldsSettings, CopyPropertiesSettings>("{T}");
                }
            }

            return errorBuilder;
        }

        private static class Throw
        {
            // ReSharper disable once UnusedParameter.Local
            internal static void CannotCopyMember<T>(Type sourceType, MemberInfo member, T settings)
                where T : CopySettings
            {
                CannotCopyMember<T>(sourceType, member);
            }

            internal static void CannotCopyMember<T>(Type sourceType, MemberInfo member)
                where T : CopySettings
            {
                var errorBuilder = new StringBuilder();
                AppendCannotCopyMember<T>(errorBuilder, sourceType, member);
                var message = errorBuilder.ToString();
                throw new NotSupportedException(message);
            }

            // ReSharper disable once UnusedParameter.Local
            internal static void AppendCannotCopyMember<T>(StringBuilder errorBuilder, Type sourceType, MemberInfo member, T settings)
                where T : CopySettings
            {
                AppendCannotCopyMember<T>(errorBuilder, sourceType, member);
            }

            internal static void AppendCannotCopyMember<T>(
                StringBuilder errorBuilder,
                Type sourceType,
                MemberInfo member)
                where T : CopySettings
            {
                errorBuilder.CreateIfNull()
                            .AppendCopyFailed<T>();
                Type memberType = null;
                var propertyInfo = member as PropertyInfo;
                if (propertyInfo != null)
                {
                    if (propertyInfo.GetIndexParameters().Length > 0)
                    {
                        errorBuilder.AppendLine($"Indexers are not supported.");
                    }

                    errorBuilder.AppendLine($"The property {sourceType.PrettyName()}.{member.Name} is not supported.");
                    errorBuilder.AppendLine($"The property is of type {propertyInfo.PropertyType.PrettyName()}.");
                    memberType = propertyInfo.PropertyType;
                }
                else
                {
                    var fieldInfo = member as FieldInfo;
                    if (fieldInfo != null)
                    {
                        errorBuilder.AppendLine($"The field {sourceType.PrettyName()}.{member.Name} is not supported.");
                        errorBuilder.AppendLine($"The field is of type {fieldInfo.FieldType.PrettyName()}.");
                        memberType = fieldInfo.FieldType;
                    }
                    else
                    {
                        Gu.ChangeTracking.Throw.ThrowThereIsABugInTheLibraryExpectedParameterOfTypes<PropertyInfo, FieldInfo>(nameof(member));
                    }
                }

                errorBuilder.AppendSolveTheProblemBy()
                            .AppendSuggestImmutableType(memberType)
                            .AppendSuggestCopySettings<T>(sourceType, member);
            }

            internal static void AppendCannotCopyIndexer<T>(
                StringBuilder errorBuilder,
                Type sourceType,
                PropertyInfo indexer)
                where T : CopySettings
            {
                Debug.Assert(indexer.GetIndexParameters().Length > 0, "Must be an indexer");
                errorBuilder.AppendCopyFailed<T>();
                errorBuilder.AppendLine($"Indexers are not supported.");
                errorBuilder.AppendLine($"The property {sourceType.PrettyName()}.{indexer.Name} is not supported.");
                errorBuilder.AppendLine($"The property is of type {indexer.PropertyType.PrettyName()}.");

                errorBuilder.AppendSolveTheProblemBy()
                            .AppendSuggestCopySettings<T>(sourceType, indexer);
            }

            // ReSharper disable once UnusedParameter.Local
            internal static void ReadonlyMemberDiffers<T>(
                SourceAndTargetValue sourceAndTargetValue,
                MemberInfo member,
                T settings)
                where T : CopySettings
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
                         Gu.ChangeTracking.Throw.ThrowThereIsABugInTheLibraryExpectedParameterOfTypes<PropertyInfo, FieldInfo>(nameof(member));
                    }
                }

                errorBuilder.AppendSolveTheProblemBy()
                    .AppendSuggestCopySettings<T>(member.DeclaringType, member);
                throw new InvalidOperationException(errorBuilder.ToString());
            }

            internal static void CannotCopyType<T>(Type type, T settings)
                where T : CopySettings
            {
                var errorBuilder = new StringBuilder();
                AppendCannotCopyType(errorBuilder, type, settings);
                throw new NotSupportedException(errorBuilder.ToString());
            }

            // ReSharper disable once UnusedParameter.Local
            internal static void AppendCannotCopyType<T>(StringBuilder errorBuilder, Type type, T settings)
                where T : CopySettings
            {
                errorBuilder.AppendCopyFailed<T>()
                            .AppendLine($"The type {type.PrettyName()} is not supported.")
                            .AppendSolveTheProblemBy()
                            .AppendSuggestImmutableType(type)
                            .AppendSuggestCopySettings<T>(type, null);
            }

            internal static void CannotCopyItem<T>(IList source, IList target, int index, T settings)
                where T : CopySettings
            {
                var itemType = source[index]?.GetType() ?? source.GetType().GetItemType();
                var errorBuilder = new StringBuilder();
                if (settings is CopyPropertiesSettings)
                {
                    errorBuilder.AppendLine(
                        $"Copy.{nameof(PropertyValues)}(x, y) does not support copying the type {itemType.PrettyName()}");
                }
                else if (settings is CopyFieldsSettings)
                {
                    errorBuilder.AppendLine(
                        $"Copy.{nameof(CopyFieldsValues)}(x, y) does not support copying the type {itemType.PrettyName()}");
                }
                else
                {
                     Gu.ChangeTracking.Throw.ThrowThereIsABugInTheLibraryExpectedParameterOfTypes<CopyPropertiesSettings, CopyFieldsSettings>(nameof(settings));
                }

                errorBuilder.AppendLine($"The problem occurred at index: {index}")
                    .AppendLine(
                        $"Source list is of type: {source.GetType().PrettyName()} and target list is of type: {target.GetType().PrettyName()}")
                    .AppendSolveTheProblemBy()
                    .AppendSuggestImplementIEquatable(itemType)
                    .AppendSuggestCopySettings<T>(itemType, null);
                throw new NotSupportedException(errorBuilder.ToString());
            }

            internal static void CannotCopyItem<T>(IDictionary source, IDictionary target, object key, T settings)
                where T : CopySettings
            {
                var itemType = source[key]?.GetType();
                var errorBuilder = new StringBuilder();
                if (settings is CopyPropertiesSettings)
                {
                    errorBuilder.AppendLine(
                        $"Copy.{nameof(PropertyValues)}(x, y) does not support copying the type {itemType.PrettyName()}");
                }
                else if (settings is CopyFieldsSettings)
                {
                    errorBuilder.AppendLine(
                        $"Copy.{nameof(CopyFieldsValues)}(x, y) does not support copying the type {itemType.PrettyName()}");
                }
                else
                {
                     Gu.ChangeTracking.Throw.ThrowThereIsABugInTheLibraryExpectedParameterOfTypes<CopyPropertiesSettings, CopyFieldsSettings>(nameof(settings));
                }

                errorBuilder.AppendLine($"The problem occurred for key: {key}")
                    .AppendLine(
                        $"Source list is of type: {source.GetType().PrettyName()} and target list is of type: {target.GetType().PrettyName()}")
                    .AppendSolveTheProblemBy()
                    .AppendSuggestImplementIEquatable(itemType)
                    .AppendSuggestCopySettings<T>(itemType, null);
                throw new NotSupportedException(errorBuilder.ToString());
            }

            // ReSharper disable once UnusedParameter.Local
            internal static void CannotCopyFixesSizeCollections<T>(ICollection source, ICollection target, T settings)
                where T : CopySettings
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
