namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal static partial class StringBuilderExt
    {
        internal static readonly string ThereIsABugInTheLibrary = "There is a bug in the library as it:";

        internal static StringBuilder AppendCreateFailed<T>(this StringBuilder errorBuilder, PropertyPath path)
        {
            if (path.Path.Count == 0)
            {
                return errorBuilder.AppendLine($"Create {typeof(T).PrettyName()} failed for type: {path.Root.Type.PrettyName()}.");
            }

            var line = path.Path.OfType<IndexItem>().Any(x => x.Index != null)
                ? $"Create {typeof(T).PrettyName()} failed for item: {path.PathString}."
                : $"Create {typeof(T).PrettyName()} failed for property: {path.PathString}.";
            return errorBuilder.AppendLine(line);
        }

        internal static StringBuilder AppendSolveTheProblemBy(this StringBuilder errorBuilder)
        {
            return errorBuilder.AppendLine("Solve the problem by any of:");
        }

        internal static StringBuilder AppendExcludeType(this StringBuilder errorBuilder, Type type)
        {
            return errorBuilder.CreateIfNull()
                               .AppendLine($"  - Exclude the type {type.PrettyName()}.");
        }

        internal static StringBuilder AppendExcludeMember(this StringBuilder errorBuilder, Type sourceType, MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                return errorBuilder;
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return errorBuilder.AppendExcludeField(sourceType, fieldInfo);
            }

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return errorBuilder.AppendExcludeProperty(sourceType, propertyInfo);
            }

            throw Throw.ThrowThereIsABugInTheLibraryExpectedParameterOfTypes<FieldInfo, PropertyInfo>(nameof(memberInfo));
        }

        internal static StringBuilder AppendExcludeProperty(this StringBuilder errorBuilder, Type sourceType, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                return errorBuilder;
            }

            return errorBuilder.CreateIfNull()
                               .AppendLine($"  - Exclude the property {sourceType.PrettyName()}.{propertyInfo.Name}.");
        }

        internal static StringBuilder AppendExcludeField(this StringBuilder errorBuilder, Type sourceType, FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                return errorBuilder;
            }

            return errorBuilder.CreateIfNull()
                               .AppendLine($"  - Exclude the field {sourceType.PrettyName()}.{fieldInfo.Name}.");
        }

        internal static StringBuilder AppendTypeIsNotSupported(this StringBuilder errorBuilder, Type type)
        {
            return errorBuilder.CreateIfNull()
                               .AppendLine($"The type {type.PrettyName()} is not supported.");
        }

        internal static StringBuilder AppendMemberIsNotSupported(this StringBuilder errorBuilder, Type sourceType, MemberInfo memberInfo)
        {
            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return errorBuilder.AppendFieldIsNotSupported(sourceType, fieldInfo);
            }

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return errorBuilder.AppendPropertyIsNotSupported(sourceType, propertyInfo);
            }

            throw Throw.ThrowThereIsABugInTheLibraryExpectedParameterOfTypes<FieldInfo, PropertyInfo>(nameof(memberInfo));
        }

        internal static StringBuilder AppendFieldIsNotSupported(this StringBuilder errorBuilder, Type sourceType, FieldInfo fieldInfo)
        {
            return errorBuilder.CreateIfNull()
                               .AppendLine($"The field {sourceType.PrettyName()}.{fieldInfo.Name} is not supported.")
                               .AppendLine($"The field is of type {fieldInfo.FieldType.PrettyName()}.");
        }

        internal static StringBuilder AppendPropertyIsNotSupported(this StringBuilder errorBuilder, Type sourceType, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                return errorBuilder;
            }

            errorBuilder = errorBuilder.CreateIfNull();
            if (propertyInfo.GetIndexParameters().Length > 0)
            {
                errorBuilder.AppendLine($"Indexers are not supported.");
            }

            return errorBuilder.AppendLine($"The property {sourceType.PrettyName()}.{propertyInfo.Name} is not supported.")
                               .AppendLine($"The property is of type {propertyInfo.PropertyType.PrettyName()}.");
        }

        internal static StringBuilder AppendSuggestionsForEnumerableLines(this StringBuilder errorBuilder, Type sourceType)
        {
            errorBuilder.AppendLine($"* Use ObservableCollection<T> or another collection type that notifies instead of {sourceType.PrettyName()}.");

            if (sourceType.Assembly != typeof(List<>).Assembly)
            {
                var line = $"* Make {sourceType.PrettyName()} implement the interfaces {typeof(INotifyCollectionChanged).Name} and {typeof(IList).Name}.";
                errorBuilder.AppendLine(line);
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestImplement<T>(this StringBuilder errorBuilder, Type sourceType)
        {
            return errorBuilder.AppendSuggestImplement(sourceType, typeof(T).PrettyName());
        }

        internal static StringBuilder AppendSuggestImplement(this StringBuilder errorBuilder, Type sourceType, string interfaceName)
        {
            var line = sourceType.Assembly == typeof(int).Assembly
                ? $"* Use a type that implements {interfaceName} instead of {sourceType.PrettyName()}."
                : $"* Implement {interfaceName} for {sourceType.PrettyName()} or use a type that does.";
            return errorBuilder.AppendLine(line);
        }

        internal static StringBuilder AppendSuggestImmutableType(this StringBuilder errorBuilder, PropertyPath propertyPath)
        {
            var lastProperty = propertyPath.Path.OfType<PropertyItem>()
                                            .LastOrDefault();
            var line = lastProperty != null
                        ? $"* Use an immutable type instead of {lastProperty.Property.PropertyType.PrettyName()}. For immutable types the following must hold:"
                        : $"* Make {propertyPath.Root.Type.PrettyName()} immutable or use an immutable type. For immutable types the following must hold:";

            errorBuilder.AppendLine(line);
            return errorBuilder.AppendImmutableConditionsLines();
        }

        internal static StringBuilder AppendSuggestImmutableType(this StringBuilder errorBuilder, Type type)
        {
            if (type.Assembly == typeof(string).Assembly)
            {
                errorBuilder.AppendLine($"* Use an immutable type instead of {type.PrettyName()}. For immutable types the following must hold:");
            }
            else
            {
                errorBuilder.AppendLine($"* Make {type.PrettyName()} immutable or use an immutable type. For immutable types the following must hold:");
            }

            return errorBuilder.AppendImmutableConditionsLines();
        }

        internal static StringBuilder AppendSuggestChangeTrackerSettings(this StringBuilder errorBuilder, Type sourceType, PropertyPath propertyPath)
        {
            var lastProperty = propertyPath.Path.OfType<PropertyItem>()
                                           .LastOrDefault();
            errorBuilder.AppendLine($"* Use {typeof(PropertiesSettings).Name} and add a specialcase for {sourceType.PrettyName()} example:");
            errorBuilder.AppendLine($"    settings.AddIgnoredType<{sourceType.PrettyName()}>()");
            if (lastProperty != null)
            {
                errorBuilder.AppendLine("    or:");
                var declaringType = lastProperty.Property.DeclaringType.PrettyName();
                errorBuilder.AppendLine($"    settings.AddIgnoredProperty(typeof({declaringType}).GetProperty(nameof({declaringType}.{lastProperty.Property.Name})))");
            }

            return errorBuilder.AppendLine($"    Note that this means that the {typeof(ChangeTracker).Name} does not track changes so you are responsible for any tracking needed.");
        }

        internal static StringBuilder CreateIfNull(this StringBuilder errorBuilder)
        {
            return errorBuilder ?? new StringBuilder();
        }

        private static StringBuilder AppendImmutableConditionsLines(this StringBuilder errorBuilder)
        {
            errorBuilder.AppendLine("  - Must be a sealed class or a struct.");
            errorBuilder.AppendLine("  - All fields and properties must be readonly.");
            errorBuilder.AppendLine("  - All field and property types must be immutable.");
            errorBuilder.AppendLine("  - All indexers must be readonly.");
            errorBuilder.AppendLine("  - Event fields are ignored.");
            return errorBuilder;
        }
    }
}
