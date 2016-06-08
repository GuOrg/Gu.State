namespace Gu.State
{
    using System;
    using System.Linq;
    using System.Text;

    public static partial class EqualBy
    {
        private static StringBuilder AppendFailed(this StringBuilder errorBuilder, string className, string methodName)
        {
            return errorBuilder.AppendLine($"{className}.{methodName}(x, y) failed.");
        }

        private static string EqualByMethodName(this IMemberSettings settings)
        {
            if (settings is FieldsSettings)
            {
                return nameof(FieldValues);
            }

            if (settings is PropertiesSettings)
            {
                return nameof(PropertyValues);
            }

            throw Throw.ExpectedParameterOfTypes<FieldsSettings, PropertiesSettings>("{T}");
        }

        private static StringBuilder AppendSuggestReferenceHandling(
            this StringBuilder errorBuilder,
            TypeErrors errors,
            IMemberSettings settings)
        {
            var references = $"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.References)} means that reference equality is used.";
            if (settings.ReferenceHandling == ReferenceHandling.Throw)
            {
                if (errors.AllErrors.OfType<RequiresReferenceHandling>().Any())
                {
                    return errorBuilder.AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Structural)} means that a deep equals is performed.")
                                       .AppendLine(references);
                }
            }

            if (errors.AllErrors.OfType<ReferenceLoop>().Any())
            {
                return errorBuilder.AppendLine(references);
            }

            return errorBuilder;
        }

        private static void ThrowIfHasErrors(this TypeErrors errors, IMemberSettings settings, string className, string methodName)
        {
            if (errors == null)
            {
                return;
            }

            if (errors.Errors.Count == 1 && ReferenceEquals(errors.Errors[0], RequiresReferenceHandling.ComplexType))
            {
                return;
            }

            var errorBuilder = new StringBuilder();
            errorBuilder.AppendFailed(className, methodName)
                        .AppendNotSupported(errors)
                        .AppendSolveTheProblemBy()
                        .AppendSuggestEquatable(errors)
                        .AppendLine($"* Use {settings.GetType().Name} and specify how comparing is performed:")
                        .AppendSuggestReferenceHandling(errors, settings)
                        .AppendSuggestExclude(errors);

            var message = errorBuilder.ToString();
            throw new NotSupportedException(message);
        }
    }
}
