namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Text;

    public static partial class Copy
    {
        private static StringBuilder AppendCopyFailed(this StringBuilder errorBuilder, string className, string methodName)
        {
            return errorBuilder.AppendLine($"{className}.{methodName}(x, y) failed.");
        }

        private static string CopyMethodName(this MemberSettings settings)
        {
            if (settings is FieldsSettings)
            {
                return nameof(FieldValues);
            }

            if (settings is PropertiesSettings)
            {
                return nameof(PropertyValues);
            }

            throw State.Throw.ExpectedParameterOfTypes<FieldsSettings, PropertiesSettings>("{T}");
        }

        // ReSharper disable once UnusedParameter.Local
        private static void ThrowIfHasErrors(this TypeErrors errors, MemberSettings settings, string className, string methodName)
        {
            if (errors == null)
            {
                return;
            }

            if (errors.Errors.Count == 1 && ReferenceEquals(errors.Errors[0], RequiresReferenceHandling.Default))
            {
                return;
            }

            var message = errors.GetErrorText(settings, className, methodName);
            throw new NotSupportedException(message);
        }

        // ReSharper disable once UnusedParameter.Local
        private static string GetErrorText(this TypeErrors errors, MemberSettings settings, string className, string methodName)
        {
            var errorBuilder = new StringBuilder();
            errorBuilder.AppendCopyFailed(className, methodName)
                        .AppendNotSupported(errors)
                        .AppendSolveTheProblemBy()
                        .AppendSuggestImmutable(errors)
                        .AppendSuggestResizableCollection(errors)
                        .AppendSuggestDefaultCtor(errors)
                        .AppendLine($"* Use {settings.GetType().Name} and specify how copying is performed:")
                        .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Structural)} means that a the entire graph is traversed and immutable property values are copied.")
                        .AppendLine($"    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.")
                        .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.References)} means that references are copied.")
                        .AppendSuggestExclude(errors);

            var message = errorBuilder.ToString();
            return message;
        }

        internal static class Throw
        {
            // ReSharper disable once UnusedParameter.Local
            internal static void ReadonlyMemberDiffers(
                SourceAndTargetValue sourceAndTargetValue,
                MemberInfo member,
                MemberSettings settings)
            {
                var error = new ReadonlyMemberDiffersError(sourceAndTargetValue, member);
                var typeErrors = new TypeErrors(sourceAndTargetValue.Source?.GetType(), error);

                var message = typeErrors.GetErrorText(settings, typeof(Copy).Name, settings.CopyMethodName());
                throw new InvalidOperationException(message);
            }

            // ReSharper disable once UnusedParameter.Local
            internal static Exception CannotCopyFixesSizeCollections(
                IEnumerable source,
                IEnumerable target,
                MemberSettings settings)
            {
                var error = new CannotCopyFixedSizeCollectionsError(source, target);
                var typeErrors = new TypeErrors(target.GetType(), error);
                var message = typeErrors.GetErrorText(settings, typeof(Copy).Name, settings.CopyMethodName());
                return new InvalidOperationException(message);
            }

            internal static InvalidOperationException CreateCannotCreateInstanceException(
                object sourceValue,
                MemberSettings settings,
                Exception exception)
            {
                var cannotCopyError = new CannotCreateInstanceError(sourceValue);
                var typeErrors = new TypeErrors(sourceValue.GetType(), new Error[] { cannotCopyError });
                var message = typeErrors.GetErrorText(settings, typeof(Copy).Name, settings.CopyMethodName());
                return new InvalidOperationException(message, exception);
            }
        }
    }
}
