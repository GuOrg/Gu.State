namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Text;

    public static partial class Copy
    {
        private static StringBuilder AppendCopyFailed(this StringBuilder errorBuilder, IMemberSettings settings)
        {
            if (settings is FieldsSettings)
            {
                return errorBuilder.AppendLine($"Copy.{nameof(Copy.FieldValues)}(x, y) failed.");
            }

            if (settings is PropertiesSettings)
            {
                return errorBuilder.AppendLine($"Copy.{nameof(Copy.PropertyValues)}(x, y) failed.");
            }

            throw State.Throw.ExpectedParameterOfTypes<FieldsSettings, PropertiesSettings>("{T}");
        }

        // ReSharper disable once UnusedParameter.Local
        private static void ThrowIfHasErrors<TSetting>(this TypeErrors errors, TSetting settings)
            where TSetting : class, IMemberSettings
        {
            if (errors == null)
            {
                return;
            }

            if (errors.Errors.Count == 1 && ReferenceEquals(errors.Errors[0], RequiresReferenceHandling.ComplexType))
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
            errorBuilder.AppendCopyFailed(settings)
                        .AppendNotSupported(errors)
                        .AppendSolveTheProblemBy()
                        .AppendSuggestImmutable(errors)
                        .AppendSuggestResizableCollection(errors)
                        .AppendSuggestDefaultCtor(errors)
                        .AppendLine($"* Use {settings.GetType() .Name} and specify how copying is performed:")
                        .AppendLine(
                            $"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Structural)} means that a the entire graph is traversed and immutable property values are copied.")
                        .AppendLine(
                            $"    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.")
                        .AppendLine(
                            $"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.References)} means that references are copied.")
                        .AppendSuggestExclude(errors);

            var message = errorBuilder.ToString();
            return message;
        }

        internal static class Throw
        {
            // ReSharper disable once UnusedParameter.Local
            internal static void ReadonlyMemberDiffers<T>(
                SourceAndTargetValue sourceAndTargetValue,
                MemberInfo member,
                T settings) where T : class, IMemberSettings
            {
                var error = new ReadonlyMemberDiffersError(sourceAndTargetValue, member);
                var typeErrors = new TypeErrors(sourceAndTargetValue.Source?.GetType(), error);

                var message = typeErrors.GetErrorText(settings);
                throw new InvalidOperationException(message);
            }

            // ReSharper disable once UnusedParameter.Local
            internal static Exception CannotCopyFixesSizeCollections<TSettings>(
                IEnumerable source,
                IEnumerable target,
                TSettings settings) where TSettings : class, IMemberSettings
            {
                var error = new CannotCopyFixedSizeCollectionsError(source, target);
                var typeErrors = new TypeErrors(target.GetType(), error);
                var message = typeErrors.GetErrorText(settings);
                return new InvalidOperationException(message);
            }

            internal static InvalidOperationException CreateCannotCreateInstanceException<TSettings>(
                object sourceValue,
                TSettings settings,
                Exception exception) where TSettings : class, IMemberSettings
            {
                var cannotCopyError = new CannotCreateInstanceError(sourceValue);
                var typeErrors = new TypeErrors(sourceValue.GetType(), new Error[] { cannotCopyError });
                var message = typeErrors.GetErrorText(settings);
                return new InvalidOperationException(message, exception);
            }
        }
    }
}
