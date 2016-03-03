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
        /// Check if the properties of <typeparamref name="T"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore properties for settings for</typeparam>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        public static void VerifyCanCopyPropertyValues<T>(
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw)
        {
            var settings = PropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
            VerifyCanCopyPropertyValues<T>(settings);
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <param name="settings">Contains configuration for how copy will be performed</param>
        public static void VerifyCanCopyPropertyValues<T>(PropertiesSettings settings)
        {
            VerifyCanCopyPropertyValues(typeof(T), settings);
        }

        /// <summary>
        /// Check if the properties of <paramref name="type"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <paramref name="type"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="settings">Contains configuration for how copy will be performed</param>
        public static void VerifyCanCopyPropertyValues(Type type, PropertiesSettings settings)
        {
            var errorBuilder = new StringBuilder();
            VerifyCanCopyPropertyValues(type, settings, errorBuilder, null);

            if (errorBuilder.Length > 0)
            {
                var message = errorBuilder.ToString();
                throw new NotSupportedException(message);
            }
        }

        private static void VerifyCanCopyPropertyValues(
            Type type,
            PropertiesSettings settings,
            StringBuilder errorBuilder,
            List<Type> checkedTypes)
        {
            Verify.Enumerable(type, settings, errorBuilder);
            Verify.Indexers(type, settings, errorBuilder);
            var propertyInfos = type.GetProperties(settings.BindingFlags);
            foreach (var propertyInfo in propertyInfos)
            {
                if (settings.IsIgnoringProperty(propertyInfo) ||
                    settings.GetSpecialCopyProperty(propertyInfo) != null)
                {
                    continue;
                }

                if (!IsCopyableType(propertyInfo.PropertyType))
                {
                    switch (settings.ReferenceHandling)
                    {
                        case ReferenceHandling.Throw:
                            errorBuilder.AppendCannotCopyMember(type, propertyInfo, settings);
                            break;
                        case ReferenceHandling.References:
                            break;
                        case ReferenceHandling.Structural:
                        case ReferenceHandling.StructuralWithReferenceLoops:
                            if (checkedTypes == null)
                            {
                                checkedTypes = new List<Type> { type };
                            }

                            if (checkedTypes.All(x => x != propertyInfo.PropertyType))
                            {
                                VerifyCanCopyPropertyValues(propertyInfo.PropertyType, settings, errorBuilder, checkedTypes);
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
