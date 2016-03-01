namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal static class ThrowHelper
    {
        private static readonly string ThereIsABugInTheLibrary = "There is a bug in the library as it:";

        internal static StringBuilder AppendCreateFailed<T>(this StringBuilder messageBuilder, PropertyPath path)
        {
            if (path.Path.Count == 0)
            {
                return messageBuilder.AppendLine($"Create {typeof(T).PrettyName()} failed for type: {path.Root.Type.PrettyName()}.");
            }

            var line = path.Path.OfType<IndexItem>().Any(x => x.Index != null)
                ? $"Create {typeof(T).PrettyName()} failed for item: {path.PathString}."
                : $"Create {typeof(T).PrettyName()} failed for property: {path.PathString}.";
            return messageBuilder.AppendLine(line);
        }

        internal static StringBuilder AppendSolveTheProblemBy(this StringBuilder messageBuilder)
        {
            return messageBuilder.AppendLine("Solve the problem by any of:");
        }

        internal static StringBuilder AppendSuggestionsForEnumerableLines(this StringBuilder messageBuilder, Type sourceType)
        {
            messageBuilder.AppendLine($"* Use ObservableCollection<T> or another collection type that notifies instead of {sourceType.PrettyName()}.");

            if (sourceType.Assembly != typeof(List<>).Assembly)
            {
                var line = $"* Make {sourceType.PrettyName()} implement the interfaces {typeof(INotifyCollectionChanged).Name} and {typeof(IList).Name}.";
                messageBuilder.AppendLine(line);
            }

            return messageBuilder;
        }

        internal static StringBuilder AppendSuggestImplement<T>(this StringBuilder messageBuilder, Type sourceType)
        {
            return messageBuilder.AppendSuggestImplement(sourceType, typeof(T).PrettyName());
        }

        internal static StringBuilder AppendSuggestImplementIEquatable(this StringBuilder messageBuilder, Type sourceType)
        {
            return messageBuilder.AppendSuggestImplement(sourceType, $"IEquatable<{sourceType.PrettyName()}>");
        }

        internal static StringBuilder AppendSuggestImmutableType(this StringBuilder messageBuilder, PropertyPath propertyPath)
        {
            var lastProperty = propertyPath.Path.OfType<PropertyItem>()
                                            .LastOrDefault();
            var line = lastProperty != null
                        ? $"* Use an immutable type instead of {lastProperty.Property.PropertyType.PrettyName()}. For immutable types the following must hold:"
                        : $"* Make {propertyPath.Root.Type.PrettyName()} immutable or use an immutable type. For immutable types the following must hold:";

            messageBuilder.AppendLine(line);
            return messageBuilder.AppendImmutableConditionsLines();
        }

        internal static StringBuilder AppendSuggestImmutableType(this StringBuilder messageBuilder, Type type)
        {
            if (type.Assembly == typeof(string).Assembly)
            {
                messageBuilder.AppendLine($"* Use an immutable type instead of {type.PrettyName()}. For immutable types the following must hold:");
            }
            else
            {
                messageBuilder.AppendLine($"* Make {type.PrettyName()} immutable or use an immutable type. For immutable types the following must hold:");
            }

            return messageBuilder.AppendImmutableConditionsLines();
        }

        internal static StringBuilder AppendSuggestChangeTrackerSettings(this StringBuilder messageBuilder, Type sourceType, PropertyPath propertyPath)
        {
            var lastProperty = propertyPath.Path.OfType<PropertyItem>()
                                           .LastOrDefault();
            messageBuilder.AppendLine($"* Use {typeof(ChangeTrackerSettings).Name} and add a specialcase for {sourceType.PrettyName()} example:");
            messageBuilder.AppendLine($"    settings.AddIgnoredType<{sourceType.PrettyName()}>()");
            if (lastProperty != null)
            {
                messageBuilder.AppendLine("    or:");
                var declaringType = lastProperty.Property.DeclaringType.PrettyName();
                messageBuilder.AppendLine($"    settings.AddIgnoredProperty(typeof({declaringType}).GetProperty(nameof({declaringType}.{lastProperty.Property.Name})))");
            }

            return messageBuilder.AppendLine($"    Note that this means that the {typeof(ChangeTracker).Name} does not track changes so you are responsible for any tracking needed.");
        }

        internal static StringBuilder AppendSuggestCopySettings<T>(this StringBuilder messageBuilder, Type type, MemberInfo member)
            where T : CopySettings
        {
            messageBuilder.AppendLine($"* Use {typeof(T).Name} and specify how copying is performed:");
            messageBuilder.AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Structural)} means that a deep copy is performed.");
            messageBuilder.AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.References)} means that references are copied.");
            messageBuilder.AppendLine($"  - Exclude the type {type.PrettyName()}.");
            if (member != null)
            {
                if (typeof(T) == typeof(CopyFieldsSettings))
                {
                    var indexer = member as PropertyInfo;
                    if (indexer == null)
                    {
                        messageBuilder.AppendLine($"  - Exclude the field {type.PrettyName()}.{member.Name}.");
                    }
                    else
                    {
                        Debug.Assert(indexer.GetIndexParameters().Length > 0, "Must be an indexer");
                    }
                }
                else if (typeof(T) == typeof(CopyPropertiesSettings))
                {
                    messageBuilder.AppendLine($"  - Exclude the property {type.PrettyName()}.{member.Name}.");
                }
                else
                {
                    ThrowThereIsABugInTheLibraryExpectedParameterOfTypes<CopyFieldsSettings, CopyPropertiesSettings>("{T}");
                }
            }

            return messageBuilder;
        }

        private static StringBuilder AppendImmutableConditionsLines(this StringBuilder messageBuilder)
        {
            messageBuilder.AppendLine("  - Must be a sealed class or a struct.");
            messageBuilder.AppendLine("  - All fields and properties must be readonly.");
            messageBuilder.AppendLine("  - All field and property types must be immutable.");
            messageBuilder.AppendLine("  - All indexers must be readonly.");
            messageBuilder.AppendLine("  - Event fields are ignored.");
            return messageBuilder;
        }

        internal static StringBuilder AppendSuggestImplement(this StringBuilder messageBuilder, Type sourceType, string interfaceName)
        {
            var line = sourceType.Assembly == typeof(int).Assembly
                ? $"* Use a type that implements {interfaceName} instead of {sourceType.PrettyName()}."
                : $"* Implement {interfaceName} for {sourceType.PrettyName()} or use a type that does.";
            return messageBuilder.AppendLine(line);
        }

        internal static void ThrowThereIsABugInTheLibraryExpectedParameterOfTypes<T1, T2>(string parameterName)
        {
            var message = $"Expected {nameof(parameterName)} to be either of {typeof(T1).PrettyName()} or {typeof(T2).PrettyName()}";
            ThrowThereIsABugInTheLibrary(message);
        }

        internal static void ThrowThereIsABugInTheLibrary(string message)
        {
            message = $"{ThrowHelper.ThereIsABugInTheLibrary}\r\n" + message;
            throw new InvalidOperationException(message);
        }
    }
}
