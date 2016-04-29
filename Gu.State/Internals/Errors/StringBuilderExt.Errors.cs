namespace Gu.State
{
    using System.Linq;
    using System.Text;

    internal static partial class StringBuilderExt
    {
        internal static StringBuilder AppendNotSupported(this StringBuilder errorBuilder, TypeErrors errors)
        {
            foreach (var error in errors.AllErrors.OfType<INotSupported>().Distinct())
            {
                error.AppendNotSupported(errorBuilder);
            }

            foreach (var member in errors.AllErrors.OfType<MemberErrors>().Select(x => x.Member).Distinct())
            {
                errorBuilder.AppendNotSupportedMember(member);
            }

            if (errors.AllErrors.OfType<UnsupportedIndexer>().Any())
            {
                errorBuilder.AppendLine("Indexers are not supported.");
                foreach (var indexer in errors.AllErrors.OfType<UnsupportedIndexer>().Select(x => x.Indexer).Distinct())
                {
                    errorBuilder.AppendNotSupportedMember(indexer);
                }
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestEquatable(this StringBuilder errorBuilder, TypeErrors errors)
        {
            const string Format = "IEquatable<{0}>";
            var types = errors.AllErrors.OfType<TypeErrors>()
                              .Select(x => x.Type)
                              .Distinct();
            foreach (var type in types)
            {
                var iEquatable = string.Format(Format, type.PrettyName());
                var line = type.Assembly == typeof(int).Assembly
                    ? $"* Use a type that implements IEquatable<> instead of {type.PrettyName()}."
                    : $"* Implement {iEquatable} for {type.PrettyName()} or use a type that does.";
                errorBuilder.AppendLine(line);
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestImmutable(this StringBuilder errorBuilder, TypeErrors errors)
        {
            var fixableTypes = errors.AllErrors.OfType<TypeErrors>()
                                     .Where(x => x.Type != errors.Type)
                                     .Select(x => x.Type)
                                     .Distinct();
            if (!fixableTypes.Any())
            {
                return errorBuilder;
            }

            foreach (var type in fixableTypes)
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

        internal static StringBuilder AppendSuggestNotify(this StringBuilder errorBuilder, TypeErrors errors)
        {
            foreach (var error in errors.AllErrors.OfType<TypeErrors>().Where(te => te.HasError<IFixWithNotify>()).Distinct())
            {
                var fixable = error.Errors.OfType<IFixWithNotify>()
                                          .First();
                fixable.AppendSuggestFixWithNotify(errorBuilder, error.Type);
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestExclude(this StringBuilder errorBuilder, TypeErrors errors)
        {
            var types = errors.AllErrors.OfType<TypeErrors>()
                              .Where(e => e.Type != errors.Type)
                              .Select(x => x.Type)
                              .Distinct();

            if (types.Any() || errors.AllErrors.OfType<IExcludableMember>().Any())
            {
                errorBuilder.AppendLine("  - Exclude a combination of the following:");
                foreach (var member in errors.AllErrors.OfType<IExcludableMember>().Select(x => x.Member).Distinct())
                {
                    errorBuilder.AppendSuggestExcludeMember(member);
                }

                foreach (var type in types)
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

        internal static StringBuilder AppendSuggestResizableCollection(this StringBuilder errorBuilder, TypeErrors errors)
        {
            foreach (var type in errors.AllErrors.OfType<CannotCopyFixesSizeCollectionsError>().Select(x => x.Type).Distinct())
            {
                errorBuilder.AppendLine($"* Use a resizable collection like List<{type.GetItemType().PrettyName()}> instead of {type.PrettyName()}.")
                            .AppendLine("* Check that the collections are the same size before calling.");
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestDefaultCtor(this StringBuilder errorBuilder, TypeErrors errors)
        {
            foreach (var type in errors.AllErrors.OfType<CannotCreateInstanceError>().Select(x => x.Type).Distinct())
            {
                errorBuilder.AppendLine($"* Add a parameterless constructor to {type.PrettyName()}, can be private.");
            }

            return errorBuilder;
        }
    }
}
