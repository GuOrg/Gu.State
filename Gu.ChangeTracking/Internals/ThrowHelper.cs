namespace Gu.ChangeTracking
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    internal static class ThrowHelper
    {
        internal static StringBuilder AppendCreateFailedForLine<T>(this StringBuilder messageBuilder, PropertyPath path)
        {
            var line = path.Path.OfType<PropertyPath.PropertyItem>().LastOrDefault() != null
                           ? $"Create {typeof(T).Name} failed for property: {path.PathString}."
                           : $"Create {typeof(T).Name} failed for type: {path.Root.Type.Name}.";
            return messageBuilder.AppendLine(line);
        }

        internal static StringBuilder AppendSolveTheProblemByLine(this StringBuilder messageBuilder)
        {
            return messageBuilder.AppendLine("Solve the problem by any of:");
        }

        internal static StringBuilder AppendImplementsIListAndINotifyCollectionChangedLines(this StringBuilder messageBuilder, object source, PropertyPath path)
        {
            var line = path.Path.OfType<PropertyPath.PropertyItem>().LastOrDefault() != null
                           ? $"* Implement {typeof(INotifyCollectionChanged).Name}  {typeof(IList).Name} and for {path.PathString} or use a type that does."
                           : $"* Make {source.GetType()} implement {typeof(INotifyCollectionChanged).Name}  {typeof(IList).Name} or use a type that does.";
            return messageBuilder.AppendLine(line);
        }

        internal static StringBuilder AppendImplementsLine<T>(this StringBuilder messageBuilder, object source, PropertyPath path)
        {
            var line = path.Path.OfType<PropertyPath.PropertyItem>().LastOrDefault() != null
                           ? $"* Implement {typeof(T).Name} for {path.PathString} or use a type that does."
                           : $"* Make {source.GetType()} implement {typeof(T).Name} or use a type that does.";
            return messageBuilder.AppendLine(line);
        }

        internal static StringBuilder AppendUseImmutableTypeLine(this StringBuilder messageBuilder, PropertyPath propertyPath)
        {
            var lastProperty = propertyPath.Path.OfType<PropertyPath.PropertyItem>()
                                            .LastOrDefault();
            var line = lastProperty != null
                        ? $"* Use an immutable type for {propertyPath.PathString}. For a class to be deemed immutable the following must hold:"
                        : $"* Use an immutable type instead of {propertyPath.Root.Type} or make it immutable if possible. For a class to be deemed immutable the following must hold:";

            messageBuilder.AppendLine(line);
            return messageBuilder.AppendImmutableConditionsLines();
        }

        internal static StringBuilder AppendImmutableConditionsLines(this StringBuilder messageBuilder)
        {
            messageBuilder.AppendLine("  - Must be sealed or struct.");
            messageBuilder.AppendLine("  - All fields and properties must be readonly.");
            messageBuilder.AppendLine("  - All fields and properties values must be immutable.");
            messageBuilder.AppendLine("  - All indexers must be readonly.");
            messageBuilder.AppendLine("  - Event fields are ignored.");
            return messageBuilder;
        }

        internal static StringBuilder AppendChangeTrackerSettingsSpecialCaseLines(this StringBuilder messageBuilder, object source, PropertyPath propertyPath)
        {
            var sourceType = source.GetType();
            var lastProperty = propertyPath.Path.OfType<PropertyPath.PropertyItem>()
                                           .LastOrDefault();
            messageBuilder.AppendLine($"* Add a special case to {typeof(ChangeTrackerSettings).Name} example:");
            messageBuilder.AppendLine($"    settings.AddExplicitType<{sourceType.PrettyName()}>()\r\n");
            if (lastProperty != null)
            {
                messageBuilder.AppendLine("    or:");
                messageBuilder.AppendLine($"    settings.AddExplicitProperty(typeof({sourceType.Name}).GetProperty(nameof({sourceType.Name}.{lastProperty.Property.Name})))");
            }

            return messageBuilder.AppendLine($"    Note that this means that the {typeof(ChangeTracker).Name} does not track changes so you are responsible for any tracking needed.");
        }
    }
}
