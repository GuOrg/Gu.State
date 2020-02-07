namespace Gu.State
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static partial class EqualBy
    {
        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be compared for equality
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore properties for settings for.</typeparam>
        /// <param name="referenceHandling">
        /// If Structural is used a deep equality check is performed.
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting properties.</param>
        public static void VerifyCanEqualByPropertyValues<T>(ReferenceHandling referenceHandling = ReferenceHandling.Structural, BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags)
        {
            var settings = PropertiesSettings.GetOrCreate(referenceHandling, bindingFlags);
            ThrowIfHasErrors(settings.GetRootEqualByComparer(typeof(T)), settings, typeof(EqualBy).Name, nameof(PropertyValues));
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be compared for equality
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <param name="settings">The settings to use.</param>
        public static void VerifyCanEqualByPropertyValues<T>(PropertiesSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            ThrowIfHasErrors(settings.GetRootEqualByComparer(typeof(T)), settings, typeof(EqualBy).Name, nameof(PropertyValues));
        }

        /// <summary>
        /// Check if the properties of <paramref name="type"/> can be compared for equality
        /// This method will throw an exception if copy cannot be performed for <paramref name="type"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="settings">The settings to use.</param>
        public static void VerifyCanEqualByPropertyValues(Type type, PropertiesSettings settings)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            ThrowIfHasErrors(settings.GetRootEqualByComparer(type), settings, typeof(EqualBy).Name, nameof(PropertyValues));
        }

        /// <summary>
        /// Check if the fields of <typeparamref name="T"/> can be compared for equality
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for.</typeparam>
        /// <param name="referenceHandling">
        /// If Structural is used a deep equality check is performed.
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting fields.</param>
        public static void VerifyCanEqualByFieldValues<T>(
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags)
        {
            var settings = FieldsSettings.GetOrCreate(referenceHandling, bindingFlags);
            ThrowIfHasErrors(settings.GetRootEqualByComparer(typeof(T)), settings, typeof(EqualBy).Name, nameof(PropertyValues));
        }

        /// <summary>
        /// Check if the fields of <typeparamref name="T"/> can be compared for equality
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <param name="settings">The settings to use.</param>
        public static void VerifyCanEqualByFieldValues<T>(FieldsSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            ThrowIfHasErrors(settings.GetRootEqualByComparer(typeof(T)), settings, typeof(EqualBy).Name, nameof(FieldValues));
        }

        /// <summary>
        /// Check if the fields of <paramref name="type"/> can be compared for equality
        /// This method will throw an exception if copy cannot be performed for <paramref name="type"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="settings">The settings to use.</param>
        public static void VerifyCanEqualByFieldValues(Type type, FieldsSettings settings)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            ThrowIfHasErrors(settings.GetRootEqualByComparer(type), settings, typeof(EqualBy).Name, nameof(FieldValues));
        }

        internal static void VerifyCanEqualByMemberValues(Type type, MemberSettings settings, string typeName, string methodName)
        {
            ThrowIfHasErrors(settings.GetRootEqualByComparer(type), settings, typeName, methodName);
        }

        private static void ThrowIfHasErrors(EqualByComparer comparer, MemberSettings settings, string className, string methodName)
        {
            if (comparer.TryGetError(settings, out var errors))
            {
                if (errors is TypeErrors typeErrors)
                {
                    var errorBuilder = new StringBuilder()
                                       .AppendLine($"{className}.{methodName}(x, y) failed.")
                                       .AppendNotSupported(typeErrors)
                                       .AppendSolveTheProblemBy()
                                       .AppendSuggestEquatable(typeErrors)
                                       .AppendLine($"* Use {settings.GetType().Name} and specify how comparing is performed:")
                                       .AppendSuggestReferenceHandling(typeErrors, settings)
                                       .AppendSuggestExclude(typeErrors);

                    var message = errorBuilder.ToString();
                    throw new NotSupportedException(message);
                }

                throw Throw.ShouldNeverGetHereException($"Expected TypeErrors was {errors}.");
            }
        }

        private static StringBuilder AppendSuggestReferenceHandling(this StringBuilder errorBuilder, TypeErrors errors, MemberSettings settings)
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
    }
}
