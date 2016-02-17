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
        /// Copies property values from source to target.
        /// </summary>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        public static void PropertyValues<T>(T source, T target, ReferenceHandling referenceHandling)
            where T : class
        {
            PropertyValues(source, target, Constants.DefaultPropertyBindingFlags, referenceHandling);
        }

        /// <summary>
        /// Copies property values from source to target.
        /// </summary>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        public static void PropertyValues<T>(T source, T target, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            where T : class
        {
            var settings = new CopyPropertiesSettings(null, null, bindingFlags, referenceHandling);
            PropertyValues(source, target, settings);
        }

        public static void PropertyValues<T>(T source, T target, params string[] excludedProperties)
            where T : class
        {
            PropertyValues(source, target, Constants.DefaultPropertyBindingFlags, excludedProperties);
        }

        /// <summary>
        /// Copies property values from source to target.
        /// Only value types and string are allowed.
        /// </summary>
        public static void PropertyValues<T>(T source, T target, BindingFlags bindingFlags, params string[] excludedProperties)
            where T : class
        {
            PropertyValues(source, target, bindingFlags, null, excludedProperties);
        }

        public static void PropertyValues<T>(T source, T target, IReadOnlyList<SpecialCopyProperty> specialCopyProperties, params string[] excludedProperties)
            where T : class
        {
            PropertyValues(source, target, Constants.DefaultPropertyBindingFlags, specialCopyProperties, excludedProperties);
        }

        /// <summary>
        /// Copies property values from source to target.
        /// Only value types and string are allowed.
        /// </summary>
        public static void PropertyValues<T>(T source, T target, BindingFlags bindingFlags, IReadOnlyList<SpecialCopyProperty> specialCopyProperties, params string[] excludedProperties)
            where T : class
        {
            var settings = new CopyPropertiesSettings(source?.GetType().GetIgnoreProperties(bindingFlags, excludedProperties), specialCopyProperties, bindingFlags, ReferenceHandling.Throw);
            PropertyValues(source, target, settings);
        }

        /// <summary>
        /// Copies property values from source to target.
        /// Only value types and string are allowed.
        /// </summary>
        public static void PropertyValues<T>(T source, T target, CopyPropertiesSettings settings)
            where T : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));
            Ensure.SameType(source, target);
            var sourceList = source as IList;
            var targetList = target as IList;
            if (sourceList != null && targetList != null)
            {
                SyncLists(sourceList, targetList, PropertyValues, settings);
                return;
            }

            Ensure.NotIs<IEnumerable>(source, nameof(source));
            var propertyInfos = source.GetType()
                                      .GetProperties(settings.BindingFlags);
            WritableProperties(source, target, propertyInfos, settings);
            VerifyReadonlyPropertiesAreEqual(source, target, propertyInfos, settings);
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
            var settings = new CopyPropertiesSettings(
                typeof(T).GetIgnoreProperties(bindingFlags, excludedProperties),
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
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                throw new NotSupportedException("Not supporting IEnumerable");
            }

            var propertyInfos = typeof(T).GetProperties(settings.BindingFlags)
                                         .Where(p => !settings.IsIgnoringProperty(p))
                                         .ToArray();

            VerifyCanCopyPropertyValues(propertyInfos);
        }

        internal static void VerifyCanCopyPropertyValues(IReadOnlyList<PropertyInfo> properties)
        {
            ////var missingSetters = properties.Where(p => p.SetMethod == null).ToArray();

            var illegalTypes = properties.Where(p => !IsCopyableType(p.PropertyType))
                                         .ToArray();

            if ( /* missingSetters.Any() || */ illegalTypes.Any())
            {
                var stringBuilder = new StringBuilder();
                ////if (missingSetters.Any())
                ////{
                ////    stringBuilder.AppendLine("Missing setters:");
                ////    foreach (var prop in missingSetters)
                ////    {
                ////        stringBuilder.AppendLine($"The property {prop.Name} does not have a setter");
                ////    }
                ////}

                if (illegalTypes.Any())
                {
                    stringBuilder.AppendLine("Illegal types:");
                    foreach (var prop in illegalTypes)
                    {
                        stringBuilder.AppendLine($"The property {prop.Name} is not of a supported type.");
                        stringBuilder.AppendLine($" Expected valuetype or string but was {prop.PropertyType}");
                    }
                }

                var message = stringBuilder.ToString();
                throw new NotSupportedException(message);
            }
        }

        internal static void WritableProperties(object source, object target, IReadOnlyList<PropertyInfo> propertyInfos, CopyPropertiesSettings settings)
        {
            foreach (var propertyInfo in propertyInfos)
            {
                if (settings.IsIgnoringProperty(propertyInfo))
                {
                    continue;
                }

                var specialCopyProperty = settings.GetSpecialCopyProperty(propertyInfo);
                if (specialCopyProperty != null)
                {
                    specialCopyProperty.CopyValue(source, target);
                    continue;
                }

                if (!propertyInfo.CanWrite)
                {
                    continue;
                }

                if (!IsCopyableType(propertyInfo.PropertyType))
                {
                    switch (settings.ReferenceHandling)
                    {
                        case ReferenceHandling.Reference:
                            break;
                        case ReferenceHandling.Structural:
                            var sourceValue = propertyInfo.GetValue(source);
                            if (sourceValue == null)
                            {
                                propertyInfo.SetValue(target, null, null);
                                continue;
                            }

                            var targetValue = propertyInfo.GetValue(target);
                            if (targetValue == null)
                            {
                                targetValue = Activator.CreateInstance(sourceValue.GetType(), true);
                                PropertyValues(sourceValue, targetValue, settings);
                                propertyInfo.SetValue(target, targetValue, null);
                            }
                            else
                            {
                                PropertyValues(sourceValue, targetValue, settings);
                            }

                            continue;
                        case ReferenceHandling.Throw:
                            var message = "Only properties with types struct or string are supported without specifying ReferenceHandling\r\n" +
                                         $"Property {source.GetType().Name}.{propertyInfo.Name} is a reference type ({propertyInfo.PropertyType.Name}).\r\n" +
                                          "Use the overload Copy.PropertyValues(source, target, ReferenceHandling) if you want to copy a graph";
                            throw new NotSupportedException(message);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(settings.ReferenceHandling), settings.ReferenceHandling, null);
                    }
                }

                var value = propertyInfo.GetValue(source);
                propertyInfo.SetValue(target, value, null);
            }
        }

        internal static void VerifyReadonlyPropertiesAreEqual(object source, object target, IReadOnlyList<PropertyInfo> propertyInfos, CopyPropertiesSettings settings)
        {
            foreach (var propertyInfo in propertyInfos)
            {
                if (settings.IsIgnoringProperty(propertyInfo) ||
                    settings.GetSpecialCopyProperty(propertyInfo) != null)
                {
                    continue;
                }

                if (propertyInfo.CanWrite)
                {
                    continue;
                }

                var sourceValue = propertyInfo.GetValue(source);
                var targetValue = propertyInfo.GetValue(target);
                if (!Equals(sourceValue, targetValue))
                {
                    var message = $"Value differs for readonly property {source.GetType().Name}.{propertyInfo.Name}";
                    throw new InvalidOperationException(message);
                }
            }
        }

        internal static void PropertyValue(object source, object target, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                return;
            }

            var sourceValue = propertyInfo.GetValue(source);
            if (propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(target, sourceValue);
            }
            else
            {
                var targetValue = propertyInfo.GetValue(target);
                if (!Equals(sourceValue, targetValue))
                {
                    var message = $"Property {propertyInfo.Name} changed but cannot be updated because it is readonly.";
                    throw new InvalidOperationException(message);
                }
            }
        }
    }
}
