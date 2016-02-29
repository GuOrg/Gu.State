namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static partial class Copy
    {
        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be synchronized.
        /// Use this to fail fast.
        /// </summary>
        public static void VerifyCanCopyPropertyValues<T>(ReferenceHandling referenceHandling)
        {
            var settings = CopyPropertiesSettings.GetOrCreate(referenceHandling);
            VerifyCanCopyPropertyValues<T>(settings);
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be synchronized.
        /// Use this to fail fast.
        /// </summary>
        public static void VerifyCanCopyPropertyValues<T>(params string[] excludedProperties)
        {
            VerifyCanCopyPropertyValues<T>(Constants.DefaultPropertyBindingFlags, excludedProperties);
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be synchronized.
        /// Use this to fail fast.
        /// </summary>
        public static void VerifyCanCopyPropertyValues<T>(BindingFlags bindingFlags, params string[] excludedProperties)
        {
            var settings = CopyPropertiesSettings.Create(
                typeof(T),
                excludedProperties,
                bindingFlags,
                ReferenceHandling.Throw);
            VerifyCanCopyPropertyValues<T>(settings);
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be synchronized.
        /// Use this to fail fast.
        /// </summary>
        public static void VerifyCanCopyPropertyValues<T>(CopyPropertiesSettings settings)
        {
            VerifyCanCopyPropertyValues(typeof(T), settings);
        }

        public static void VerifyCanCopyPropertyValues(Type type, CopyPropertiesSettings settings)
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
            CopyPropertiesSettings settings,
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
                            Throw.AppendCannotCopyMember(errorBuilder, type, propertyInfo, settings);
                            break;
                        case ReferenceHandling.References:
                            break;
                        case ReferenceHandling.Structural:
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
