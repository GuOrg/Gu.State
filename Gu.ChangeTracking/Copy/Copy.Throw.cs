namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    public static partial class Copy
    {
        private static StringBuilder AppendCopyFailed<T>(this StringBuilder errorBuilder)
            where T : IMemberSettings
        {
            if (typeof(FieldsSettings).IsAssignableFrom(typeof(T)))
            {
                return errorBuilder.CreateIfNull()
                                   .AppendLine($"Copy.{nameof(Copy.FieldValues)}(x, y) failed.");
            }

            if (typeof(PropertiesSettings).IsAssignableFrom(typeof(T)))
            {
                return errorBuilder.CreateIfNull()
                                   .AppendLine($"Copy.{nameof(Copy.PropertyValues)}(x, y) failed.");
            }

            throw ChangeTracking.Throw.ExpectedParameterOfTypes<FieldsSettings, PropertiesSettings>("{T}");
        }

        // ReSharper disable once UnusedParameter.Local
        private static void ThrowIfHasErrors<TSetting>(this TypeErrors errors, TSetting settings)
            where TSetting : class, IMemberSettings
        {
            if (errors == null)
            {
                return;
            }

            var message = errors.GetErrorText(settings);
            throw new NotSupportedException(message);
        }

        // ReSharper disable once UnusedParameter.Local
        private static string GetErrorText<TSettings>(this TypeErrors errors, TSettings settings)
            where TSettings : class, IMemberSettings
        {
            var errorBuilder = new StringBuilder();
            errorBuilder.AppendCopyFailed<TSettings>()
                        .AppendNotSupported(errors)
                        .AppendSolveTheProblemBy()
                        .AppendSuggestImmutable(errors)
                        .AppendSuggestResizableCollection(errors)
                        .AppendSuggestDefaultCtor(errors)
                        .AppendLine($"* Use {typeof(TSettings).Name} and specify how copying is performed:")
                        .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Structural)} means that a the entire graph is traversed and immutable property values are copied.")
                        .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.StructuralWithReferenceLoops)} same as Structural but tracks reference loops.")
                        .AppendLine($"    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.")
                        .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.References)} means that references are copied.")
                        .AppendSuggestExclude(errors);

            var message = errorBuilder.ToString();
            return message;
        }

        private static class Throw
        {
            // ReSharper disable once UnusedParameter.Local
            internal static void ReadonlyMemberDiffers<T>(
                SourceAndTargetValue sourceAndTargetValue,
                MemberInfo member,
                T settings)
                where T : class, IMemberSettings
            {
                var typeErrors = new TypeErrors(sourceAndTargetValue.Source?.GetType())
                                     {
                                         new ReadonlyMemberDiffersError(sourceAndTargetValue, member)
                                     };

                var message = typeErrors.GetErrorText(settings);
                throw new InvalidOperationException(message);
            }

            // ReSharper disable once UnusedParameter.Local
            internal static void CannotCopyFixesSizeCollections<TSettings>(IEnumerable source, IEnumerable target, TSettings settings)
                where TSettings : class, IMemberSettings
            {
                var typeErrors = new TypeErrors(null)
                                     {
                                         new CannotCopyFixesSizeCollectionsError(source, target)
                                     };

                var message = typeErrors.GetErrorText(settings);
                throw new InvalidOperationException(message);
            }

            internal static InvalidOperationException CreateCannotCreateInstanceException<TSettings>(object sourceValue, MemberInfo member, TSettings settings, Exception exception)
                                where TSettings : class, IMemberSettings
            {
                var typeErrors = new TypeErrors(null)
                                     {
                                         new CannotCreateInstanceError(sourceValue, member)
                                     };
                var message = typeErrors.GetErrorText(settings);
                return new InvalidOperationException(message, exception);
            }
        }
    }
}
