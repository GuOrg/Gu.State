namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
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
            var settings = new CopyPropertiesSettings((IEnumerable<PropertyInfo>)null, null, bindingFlags, referenceHandling);
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
            VerifyCanCopyPropertyValues(typeof(T), settings);
        }

        public static void VerifyCanCopyPropertyValues(Type type, CopyPropertiesSettings settings)
        {
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                if (settings.ReferenceHandling == ReferenceHandling.Throw || !typeof(IList).IsAssignableFrom(type))
                {
                    throw new NotSupportedException("Collections must be : IList and ReferenceHandling must be other than Throw");
                }
            }

            var propertyInfos = type.GetProperties(settings.BindingFlags);
            var stringBuilder = new StringBuilder();
            foreach (var propertyInfo in propertyInfos)
            {
                if (settings.IsIgnoringProperty(propertyInfo) ||
                    settings.GetSpecialCopyProperty(propertyInfo) != null)
                {
                    continue;
                }

                if (settings.ReferenceHandling == ReferenceHandling.Throw && !IsCopyableType(propertyInfo.PropertyType))
                {
                    stringBuilder.AppendLine($"The property {type.Name}.{propertyInfo.Name} is not of a supported type.");
                    stringBuilder.AppendLine($"Expected struct or string but was: {propertyInfo.PropertyType.Name}");
                    stringBuilder.AppendLine($"Specify {typeof(ReferenceHandling).Name} if you want to copy a graph.");
                }
            }

            if (stringBuilder.Length > 0)
            {
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
                    if (IsCopyableType(propertyInfo.PropertyType))
                    {
                        continue;
                    }
                }

                if (!IsCopyableType(propertyInfo.PropertyType))
                {
                    switch (settings.ReferenceHandling)
                    {
                        case ReferenceHandling.Reference:
                            {
                                var value = propertyInfo.GetValue(source);
                                propertyInfo.SetValue(target, value);
                                break;
                            }
                        case ReferenceHandling.Structural:
                            var sourceValue = propertyInfo.GetValue(source);
                            if (sourceValue == null)
                            {
                                propertyInfo.SetValue(target, null);
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
                else
                {
                    var value = propertyInfo.GetValue(source);
                    propertyInfo.SetValue(target, value);
                }
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
                if (sourceValue == null && targetValue == null)
                {
                    continue;
                }

                if (!EqualBy.PropertyValues(sourceValue, targetValue, settings))
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
