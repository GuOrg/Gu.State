namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static class Copy
    {
        private static readonly BindingFlags BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Copies field values from source to target.
        /// Only valur types and string are allowed.
        /// </summary>
        public static void FieldValues<T>(T source, T target, params string[] ignoredFields)
            where T : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));
            Ensure.SameType(source, target);
            Ensure.NotIs<IEnumerable>(source, nameof(source));

            var fieldInfos = typeof(T).GetFields(BindingFlags);
            foreach (var fieldInfo in fieldInfos)
            {
                if (ignoredFields?.Contains(fieldInfo.Name) == true)
                {
                    continue;
                }

                if (IsEventField(fieldInfo))
                {
                    continue;
                }

                if (!IsCopyableType(fieldInfo.FieldType))
                {
                    var message = $"Copy does not support copying the property {fieldInfo.Name} of type {fieldInfo.FieldType}";
                    throw new NotSupportedException(message);
                }

                var sourceValue = fieldInfo.GetValue(source);
                if (fieldInfo.IsInitOnly)
                {
                    var targetValue = fieldInfo.GetValue(target);
                    if (!Equals(sourceValue, targetValue))
                    {
                        var message = $"Property {fieldInfo.Name} differs but cannot be updated because it is readonly.";
                        throw new InvalidOperationException(message);
                    }
                }
                else
                {
                    fieldInfo.SetValue(target, sourceValue);
                }
            }
        }

        public static void PropertyValues<T>(T source, T target, params string[] ignoredProperties)
            where T : class
        {
            Copy.PropertyValues(source, target, BindingFlags.Instance | BindingFlags.Public, ignoredProperties);
        }

        /// <summary>
        /// Copies property values from source to target.
        /// Only valur types and string are allowed.
        /// </summary>
        public static void PropertyValues<T>(T source, T destination, BindingFlags bindingFlags, params string[] ignoreProperties)
            where T : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(destination, nameof(destination));
            Ensure.SameType(source, destination);
            Ensure.NotIs<IEnumerable>(source, nameof(source));

            var propertyInfos = typeof(T).GetProperties(bindingFlags);
            foreach (var propertyInfo in propertyInfos)
            {
                if (ignoreProperties?.Contains(propertyInfo.Name) == true)
                {
                    continue;
                }

                if (!IsCopyableType(propertyInfo.PropertyType))
                {
                    var message = $"Copy does not support copying the property {propertyInfo.Name} of type {propertyInfo.PropertyType}";
                    throw new NotSupportedException(message);
                }

                var value = propertyInfo.GetValue(source);
                propertyInfo.SetValue(destination, value, null);
            }
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be synchronized.
        /// Use this to fail fast.
        /// </summary>
        public static void VerifyCanCopyPropertyValues<T>(params string[] ignoreProperties)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                throw new NotSupportedException("Not supporting IEnumerable");
            }

            var propertyInfos = typeof(T).GetProperties()
                .Where(p => ignoreProperties?.All(pn => pn != p.Name) == true)
                .ToArray();

            VerifyCanCopyPropertyValues<T>(propertyInfos);
        }

        /// <summary>
        /// Check if the fields of <typeparamref name="T"/> can be synchronized.
        /// Use this to fail fast.
        /// </summary>
        public static void VerifyCanCopyFieldValues<T>(params string[] ignoreFields)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                throw new NotSupportedException("Not supporting IEnumerable");
            }

            var fieldInfos = typeof(T).GetFields(BindingFlags)
                .Where(f => ignoreFields?.All(pn => pn != f.Name) == true && !IsEventField(f))
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

        internal static void VerifyCanCopyPropertyValues<T>(IReadOnlyList<PropertyInfo> properties)
        {
            var missingSetters = properties.Where(p => p.SetMethod == null).ToArray();

            var illegalTypes = properties.Where(p => !IsCopyableType(p.PropertyType))
                .ToArray();

            if (missingSetters.Any() || illegalTypes.Any())
            {
                var stringBuilder = new StringBuilder();
                if (missingSetters.Any())
                {
                    stringBuilder.AppendLine("Missing setters:");
                    foreach (var prop in missingSetters)
                    {
                        stringBuilder.AppendLine($"The property {prop.Name} does not have a setter");
                    }
                }

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

        private static bool IsEventField(FieldInfo field)
        {
            return typeof(MulticastDelegate).IsAssignableFrom(field.FieldType);
        }
    }
}