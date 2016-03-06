namespace Gu.ChangeTracking
{
    using System;
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
                throw Gu.ChangeTracking.Throw.ExpectedParameterOfTypes<PropertiesSettings, FieldsSettings>("T");
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

            var errorBuilder = new StringBuilder();
            errorBuilder.AppendEqualByFailed<TSetting>()
                        .AppendNotSupported(errors)
                        .AppendSolveTheProblemBy()
                        .AppendSuggestEquatable(errors)
                        .AppendLine($"* Use {typeof(TSetting).Name} and specify how comparing is performed:")
                        .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Structural)} means that a deep equals is performed.")
                        .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.StructuralWithReferenceLoops)} same as Structural but handles reference loops.")
                        .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.References)} means that reference equality is used.")
                        .AppendSuggestExclude(errors);

            var message = errorBuilder.ToString();
            throw new NotSupportedException(message);
        }
    }
}
