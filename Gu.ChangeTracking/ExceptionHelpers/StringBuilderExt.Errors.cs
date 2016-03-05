namespace Gu.ChangeTracking
{
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
    }
}
