namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static partial class Copy
    {
        /// <summary>
        /// Check if the fields of <typeparamref name="T"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for</typeparam>
        /// <param name="bindingFlags">The binding flags to use when getting fields</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        public static void VerifyCanCopyFieldValues<T>(
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw)
        {
            var settings = FieldsSettings.GetOrCreate(bindingFlags, referenceHandling);
            VerifyCanCopyFieldValues<T>(settings);
        }

        /// <summary>
        /// Check if the fields of <typeparamref name="T"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for</typeparam>
        /// <param name="settings">Contains configuration for how copy is performed</param>
        public static void VerifyCanCopyFieldValues<T>(FieldsSettings settings)
        {
            VerifyCanCopyFieldValues(typeof(T), settings);
        }

        /// <summary>
        /// Check if the fields of <paramref name="type"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <paramref name="type"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <param name="type">The type to get ignore fields for settings for</param>
        /// <param name="settings">Contains configuration for how copy is performed</param>
        public static void VerifyCanCopyFieldValues(Type type, FieldsSettings settings)
        {
            var errorBuilder = new StringBuilder();
            VerifyCanCopyFieldValues(type, settings, errorBuilder, null);
            if (errorBuilder.Length > 0)
            {
                var message = errorBuilder.ToString();
                throw new NotSupportedException(message);
            }
        }

        private static void VerifyCanCopyFieldValues(
            Type type,
            FieldsSettings settings,
            StringBuilder errorBuilder,
            List<Type> checkedTypes)
        {
            Verify.Enumerable(type, settings, errorBuilder);
            Verify.Indexers(type, settings, errorBuilder);

            var fieldInfos = type.GetFields(settings.BindingFlags);
            foreach (var fieldInfo in fieldInfos)
            {
                if (settings.IsIgnoringField(fieldInfo))
                {
                    continue;
                }

                if (!IsCopyableType(fieldInfo.FieldType))
                {
                    switch (settings.ReferenceHandling)
                    {
                        case ReferenceHandling.Throw:
                            Copy.AppendCannotCopyMember(errorBuilder, type, fieldInfo, settings);
                            break;
                        case ReferenceHandling.References:
                            break;
                        case ReferenceHandling.Structural:
                        case ReferenceHandling.StructuralWithReferenceLoops:
                            if (checkedTypes == null)
                            {
                                checkedTypes = new List<Type> { type };
                            }

                            if (checkedTypes.All(x => x != fieldInfo.FieldType))
                            {
                                VerifyCanCopyFieldValues(fieldInfo.FieldType, settings, errorBuilder, checkedTypes);
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}
