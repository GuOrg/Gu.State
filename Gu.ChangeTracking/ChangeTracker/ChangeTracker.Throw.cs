namespace Gu.ChangeTracking
{
    using System;
    using System.Text;

    public partial class ChangeTracker
    {
        private static class Throw
        {
            // ReSharper disable once UnusedParameter.Local
            internal static void IfHasErrors<TSetting>(TypeErrors errors, TSetting settings)
                where TSetting : class, IMemberSettings
            {
                if (errors == null)
                {
                    return;
                }

                var message = GetErrorText(errors, settings);
                throw new NotSupportedException(message);
            }

            // ReSharper disable once UnusedParameter.Local
            internal static string GetErrorText<TSettings>(TypeErrors errors, TSettings settings)
                where TSettings : class, IMemberSettings
            {
                var errorBuilder = new StringBuilder();
                errorBuilder.AppendLine("Track changes failed.")
                            .AppendNotSupported(errors)
                            .AppendSolveTheProblemBy()
                            .AppendSuggestNotify(errors)
                            .AppendSuggestImmutable(errors)
                            .AppendSuggestResizableCollection(errors)
                            .AppendSuggestDefaultCtor(errors)
                            .AppendLine($"* Use {typeof(TSettings).Name} and specify how change tracking is performed:")
                            .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Structural)} means that a the entire graph is tracked.")
                            .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.StructuralWithReferenceLoops)} same as Structural but handles reference loops.")
                            .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.References)} means that only the root level changes are tracked.")
                            .AppendSuggestExclude(errors);

                var message = errorBuilder.ToString();
                return message;
            }
        }
    }
}
