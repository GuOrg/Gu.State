namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;

    internal static class ThrowHelper
    {
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

        internal static StringBuilder AppendSuggestionsForEnumerableLines(this StringBuilder messageBuilder, Type sourceType, PropertyPath path)
        {
            messageBuilder.AppendLine($"* Use ObservableCollection<T> or another collection type that notifies instead of {sourceType.PrettyName()}.");

            if (sourceType.Assembly != typeof(List<>).Assembly)
            {
                var line = $"* Make {sourceType.PrettyName()} implement the interfaces {typeof(INotifyCollectionChanged).Name} and {typeof(IList).Name}.";
                messageBuilder.AppendLine(line);
            }

            return messageBuilder;
        }

        internal static StringBuilder AppendSuggestImplement<T>(this StringBuilder messageBuilder, Type sourceType, PropertyPath path)
        {
            var line = sourceType.Assembly == typeof(int).Assembly
                ? $"* Use a type that implements {typeof(T).PrettyName()} instead of {sourceType.PrettyName()}."
                : $"* Implement {typeof(T).PrettyName()} for {sourceType.PrettyName()} or use a type that notifies.";
            return messageBuilder.AppendLine(line);
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

        internal static StringBuilder AppendImmutableConditionsLines(this StringBuilder messageBuilder)
        {
            messageBuilder.AppendLine("  - Must be a sealed class or a struct.");
            messageBuilder.AppendLine("  - All fields and properties must be readonly.");
            messageBuilder.AppendLine("  - All field and property types must be immutable.");
            messageBuilder.AppendLine("  - All indexers must be readonly.");
            messageBuilder.AppendLine("  - Event fields are ignored.");
            return messageBuilder;
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
    }
}
