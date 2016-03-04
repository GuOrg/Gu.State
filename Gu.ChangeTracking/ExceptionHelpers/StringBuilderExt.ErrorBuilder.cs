namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal static partial class StringBuilderExt
    {
        internal static StringBuilder AppendSuggestFixFor<TError>(this StringBuilder errorBuilder, TypeErrors errors, Func<StringBuilder, TError, StringBuilder> fix)
            where TError : Error
        {
            foreach (var up in errors.OfType<TError>())
            {
                errorBuilder = fix(errorBuilder.CreateIfNull(), up);
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestExcludeTypes(this StringBuilder errorBuilder, TypeErrors errors)
        {
            var types = errors.OfType<TypeErrors>().Select(x => x.Type);
            var memberTypes = errors.OfType<MemberError>().Select(x => x.MemberInfo.DeclaringType).Where(t => t != null);
            types = types.Concat(memberTypes)
                         .Distinct();
            foreach (var type in types)
            {
                errorBuilder = errorBuilder.CreateIfNull()
                                           .AppendExcludeType(type);
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestExclude(this StringBuilder errorBuilder, TypeErrors errors, IMemberSettings settings)
        {
            if (errors.OfType<IExcludable>().Any())
            {
                errorBuilder.AppendLine("  - Exclude any or all of the following:");
                foreach (var error in errors.OfType<IExcludable>())
                {
                    error.AppendSuggestExclude(errorBuilder);
                }
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendErrors(this StringBuilder errorBuilder, TypeErrors errors)
        {
            foreach (var error in errors.OfType<MemberError>())
            {
                error.AppendNotSupported(errorBuilder);
            }

            if (errors.OfType<UnsupportedIndexer>()
                      .Any())
            {
                errorBuilder.AppendLine("Indexers are not supported.");
                foreach (var error in errors.OfType<UnsupportedIndexer>())
                {
                    error.AppendNotSupported(errorBuilder);
                }
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendUnsupportedIndexers(this StringBuilder errorBuilder, Type type, IEnumerable<PropertyInfo> unSupportedIndexers)
        {
            if (unSupportedIndexers == null || !unSupportedIndexers.Any())
            {
                return errorBuilder;
            }

            errorBuilder = errorBuilder.CreateIfNull().AppendLine($"Indexers are not supported.");

            foreach (var property in unSupportedIndexers)
            {
                errorBuilder.AppendLine($"The property {type.PrettyName()}.{property.Name} of type {property.PropertyType.PrettyName()} is an indexer and not supported.");
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestEquatable(this StringBuilder errorBuilder, TypeErrors errors)
        {
            const string format = "IEquatable<{0}>";
            foreach (var type in errors.OfType<IFixWithEquatable>().Select(x => x.Type).Distinct())
            {
                var iEquatable = string.Format(format, type.PrettyName());
                var line = type.Assembly == typeof(int).Assembly
                    ? $"* Use a type that implements {iEquatable} instead of {type.PrettyName()}."
                    : $"* Implement {iEquatable}> for {type.PrettyName()} or use a type that does.";
                errorBuilder.AppendLine(line);
            }

            return errorBuilder;
        }
    }
}
