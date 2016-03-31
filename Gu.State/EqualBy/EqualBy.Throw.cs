namespace Gu.State
{
    using System;
    using System.Linq;
    using System.Text;

    public static partial class EqualBy
    {
        private static StringBuilder AppendEqualByFailed<T>(this StringBuilder errorBuilder)
            where T : class, IMemberSettings
        {
            if (typeof(PropertiesSettings).IsAssignableFrom(typeof(T)))
            {
                errorBuilder.AppendLine($"EqualBy.{nameof(PropertyValues)}(x, y) failed.");
            }
            else if (typeof(FieldsSettings).IsAssignableFrom(typeof(T)))
            {
                errorBuilder.AppendLine($"EqualBy.{nameof(FieldValues)}(x, y) failed.");
            }
            else
            {
                throw Throw.ExpectedParameterOfTypes<PropertiesSettings, FieldsSettings>("T");
            }

            return errorBuilder;
        }

        private static StringBuilder AppendSuggestReferenceHandling(
            this StringBuilder errorBuilder,
            TypeErrors errors,
            IMemberSettings settings)
        {
            var structuralWithReferenceLoops = $"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.StructuralWithReferenceLoops)} same as Structural but handles reference loops.";
            var references = $"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.References)} means that reference equality is used.";
            if (settings.ReferenceHandling == ReferenceHandling.Throw)
            {
                if (errors.AllErrors.OfType<RequiresReferenceHandling>().Any())
                {
                    return errorBuilder.AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Structural)} means that a deep equals is performed.")
                                       .AppendLine(structuralWithReferenceLoops)
                                       .AppendLine(references);
                }
            }

            if (errors.AllErrors.OfType<ReferenceLoop>().Any())
            {
                return errorBuilder.AppendLine(structuralWithReferenceLoops)
                                   .AppendLine(references);
            }

            return errorBuilder;
        }

        // ReSharper disable once UnusedParameter.Local
        private static void ThrowIfHasErrors<TSetting>(this TypeErrors errors, TSetting settings)
            where TSetting : class, IMemberSettings
        {
            if (errors == null)
            {
                return;
            }

            if (errors.Errors.Count == 1 && ReferenceEquals(errors.Errors.Single(), RequiresReferenceHandling.Other))
            {
                return;
            }

            var errorBuilder = new StringBuilder();
            errorBuilder.AppendEqualByFailed<TSetting>()
                        .AppendNotSupported(errors)
                        .AppendSolveTheProblemBy()
                        .AppendSuggestEquatable(errors)
                        .AppendLine($"* Use {typeof(TSetting).Name} and specify how comparing is performed:")
                        .AppendSuggestReferenceHandling(errors, settings)
                        .AppendSuggestExclude(errors);

            var message = errorBuilder.ToString();
            throw new NotSupportedException(message);
        }
    }
}
