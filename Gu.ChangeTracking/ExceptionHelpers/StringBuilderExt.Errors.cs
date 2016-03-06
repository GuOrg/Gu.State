namespace Gu.ChangeTracking
{
    using System;
    using System.Linq;
    using System.Text;

    internal static partial class StringBuilderExt
    {
        internal static StringBuilder AppendSuggestExclude(this StringBuilder errorBuilder, TypeErrors errors)
        {
            if (errors.OfType<IExcludableType>().Any() ||
                errors.OfType<IExcludableMember>().Any())
            {
                errorBuilder.AppendLine("  - Exclude a combination of the following:");
                foreach (var excludableMember in errors.OfType<IExcludableMember>().Distinct())
                {
                    excludableMember.AppendSuggestExclude(errorBuilder);
                }

                foreach (var type in errors.OfType<IExcludableType>().Select(x => x.Type).Distinct())
                {
                    if (type == errors.Type)
                    {
                        continue;
                    }

                    errorBuilder.AppendLine($"    - The type {type.PrettyName()}.");
                }
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendNotSupported(this StringBuilder errorBuilder, TypeErrors errors)
        {
            foreach (var error in errors.OfType<INotSupported>().Distinct())
            {
                if (error is UnsupportedIndexer)
                {
                    continue;
                }

                error.AppendNotSupported(errorBuilder);
            }

            if (errors.OfType<UnsupportedIndexer>().Any())
            {
                errorBuilder.AppendLine("Indexers are not supported.");
                foreach (var error in errors.OfType<UnsupportedIndexer>().Distinct())
                {
                    error.AppendNotSupported(errorBuilder);
                }
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestEquatable(this StringBuilder errorBuilder, TypeErrors errors)
        {
            const string Format = "IEquatable<{0}>";
            foreach (var type in errors.OfType<IFixWithEquatable>().Select(x => x.Type).Distinct())
            {
                var iEquatable = string.Format(Format, type.PrettyName());
                var line = type.Assembly == typeof(int).Assembly
                    ? $"* Use a type that implements {iEquatable} instead of {type.PrettyName()}."
                    : $"* Implement {iEquatable} for {type.PrettyName()} or use a type that does.";
                errorBuilder.AppendLine(line);
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestImmutable(this StringBuilder errorBuilder, TypeErrors errors)
        {
            var immutables = errors.OfType<IFixWithImmutable>()
                                   .Select(x => x.Type)
                                   .Where(t => t != errors.Type)
                                   .Distinct()
                                   .ToArray();
            if (immutables.Length == 0)
            {
                return errorBuilder;
            }

            foreach (var type in immutables)
            {
                var line = type.Assembly == typeof(int).Assembly
                    ? $"* Use an immutable type instead of {type.PrettyName()}."
                    : $"* Make {type.PrettyName()} immutable or use an immutable type.";
                errorBuilder.AppendLine(line);
            }

            errorBuilder.AppendLine("  - For immutable types the following must hold:")
                        .AppendLine("    - Must be a sealed class or a struct.")
                        .AppendLine("    - All fields and properties must be readonly.")
                        .AppendLine("    - All field and property types must be immutable.")
                        .AppendLine("    - All indexers must be readonly.")
                        .AppendLine("    - Event fields are ignored.");
            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestResizableCollection(this StringBuilder errorBuilder, TypeErrors errors)
        {
            foreach (var type in errors.OfType<CannotCopyFixesSizeCollectionsError>().Select(x => x.Type).Distinct())
            {
                errorBuilder.AppendLine($"* Use a resizable collection like List<{type.GetItemType().PrettyName()}> instead of {type.PrettyName()}.")
                            .AppendLine("* Check that the collections are the same size before calling.");
            }

            return errorBuilder;
        }
    }
}
