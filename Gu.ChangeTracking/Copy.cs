namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static class Copy
    {
        /// <summary>
        /// Copies field values from source to target.
        /// Only valur types and string are allowed.
        /// </summary>
        public static void FieldValues<T>(T source, T target, params string[] excludedFields)
            where T : class
        {
            FieldValues(source, target, Constants.DefaultFieldBindingFlags, excludedFields);
        }

        /// <summary>
        /// Copies field values from source to target.
        /// Only valur types and string are allowed.
        /// </summary>
        public static void FieldValues<T>(T source, T target, BindingFlags bindingFlags, params string[] excludedFields)
            where T : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));
            Ensure.SameType(source, target);
            Ensure.NotIs<IEnumerable>(source, nameof(source));

            var fieldInfos = typeof(T).GetFields(bindingFlags);
            foreach (var fieldInfo in fieldInfos)
            {
                if (excludedFields?.Contains(fieldInfo.Name) == true)
                {
                    continue;
                }

                if (fieldInfo.IsEventField())
                {
                    continue;
                }

                if (!IsCopyableType(fieldInfo.FieldType))
                {
                    var message = $"Copy does not support copying the field {fieldInfo.Name} of type {fieldInfo.FieldType}";
                    throw new NotSupportedException(message);
                }

                var sourceValue = fieldInfo.GetValue(source);
                if (fieldInfo.IsInitOnly)
                {
                    var targetValue = fieldInfo.GetValue(target);
                    if (!Equals(sourceValue, targetValue))
                    {
                        var message = $"Field {typeof(T).Name}.{fieldInfo.Name} differs but cannot be updated because it is readonly.\r\n" +
                                      $"Provide {typeof(Copy).Name}.{nameof(FieldValues)}(x, y, nameof({typeof(T).Name}.{fieldInfo.Name}))";
                        throw new InvalidOperationException(message);
                    }
                }
                else
                {
                    fieldInfo.SetValue(target, sourceValue);
                }
            }
        }

        /// <summary>
        /// Check if the fields of <typeparamref name="T"/> can be synchronized.
        /// Use this to fail fast.
        /// </summary>
        public static void VerifyCanCopyFieldValues<T>(params string[] excludedFields)
        {
            VerifyCanCopyFieldValues<T>(Constants.DefaultFieldBindingFlags, excludedFields);
        }

        /// <summary>
        /// Check if the fields of <typeparamref name="T"/> can be synchronized.
        /// Use this to fail fast.
        /// </summary>
        public static void VerifyCanCopyFieldValues<T>(BindingFlags bindingFlags, params string[] ignoreFields)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                throw new NotSupportedException("Not supporting IEnumerable");
            }

            var fieldInfos = typeof(T).GetFields(bindingFlags)
                .Where(f => ignoreFields?.All(pn => pn != f.Name) == true && !f.IsEventField())
                .ToArray();

            var illegalTypes = fieldInfos.Where(p => !IsCopyableType(p.FieldType))
                .ToArray();

            if (illegalTypes.Any())
            {
                var stringBuilder = new StringBuilder();
                if (illegalTypes.Any())
                {
                    stringBuilder.AppendLine("Illegal types:");
                    foreach (var fieldInfo in illegalTypes)
                    {
                        stringBuilder.AppendLine($"The field {fieldInfo.Name} is not of a supported type. Expected valuetype of string but was {fieldInfo.FieldType}");
                    }
                }

                var message = stringBuilder.ToString();
                throw new NotSupportedException(message);
            }
        }

        public static void PropertyValues<T>(T source, T target, params string[] excludedProperties)
            where T : class
        {
            PropertyValues(source, target, Constants.DefaultPropertyBindingFlags, excludedProperties);
        }

        /// <summary>
        /// Copies property values from source to target.
        /// Only valur types and string are allowed.
        /// </summary>
        public static void PropertyValues<T>(
            T source,
            T target,
            BindingFlags bindingFlags,
            params string[] excludedProperties)
            where T : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));
            Ensure.SameType(source, target);
            Ensure.NotIs<IEnumerable>(source, nameof(source));

            var propertyInfos = typeof(T).GetProperties(bindingFlags);
            WritableProperties(source, target, propertyInfos, excludedProperties);
            VerifyReadonlyPropertiesAreEqual(source, target, propertyInfos, excludedProperties);
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
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                throw new NotSupportedException("Not supporting IEnumerable");
            }

            var propertyInfos = typeof(T).GetProperties(bindingFlags)
                .Where(p => excludedProperties?.All(pn => pn != p.Name) == true)
                .ToArray();

            VerifyCanCopyPropertyValues(propertyInfos);
        }

        internal static void VerifyCanCopyPropertyValues(IReadOnlyList<PropertyInfo> properties)
        {
            ////var missingSetters = properties.Where(p => p.SetMethod == null).ToArray();

            var illegalTypes = properties.Where(p => !IsCopyableType(p.PropertyType))
                .ToArray();

            if (/* missingSetters.Any() || */ illegalTypes.Any())
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
                        stringBuilder.AppendLine($"The property {prop.Name} is not of a supported type. Expected valuetype of string but was {prop.PropertyType}");
                    }
                }

                var message = stringBuilder.ToString();
                throw new NotSupportedException(message);
            }
        }

        internal static void WritableProperties(
            object source,
            object target,
            IReadOnlyList<PropertyInfo> propertyInfos,
            string[] ignoreProperties)
        {
            foreach (var propertyInfo in propertyInfos)
            {
                if (ignoreProperties?.Contains(propertyInfo.Name) == true)
                {
                    continue;
                }

                if (!propertyInfo.CanWrite)
                {
                    continue;
                }

                if (!IsCopyableType(propertyInfo.PropertyType))
                {
                    var message = $"Copy does not support copying the property {propertyInfo.Name} of type {propertyInfo.PropertyType}";
                    throw new NotSupportedException(message);
                }

                var value = propertyInfo.GetValue(source);
                propertyInfo.SetValue(target, value, null);
            }
        }

        internal static void VerifyReadonlyPropertiesAreEqual(
            object source,
            object target,
            IReadOnlyList<PropertyInfo> propertyInfos,
            string[] excludedProperties)
        {
            foreach (var propertyInfo in propertyInfos)
            {
                if (excludedProperties?.Contains(propertyInfo.Name) == true)
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

        private static bool IsCopyableType(Type type)
        {
            return type.IsValueType || type == typeof(string);
        }
    }
}