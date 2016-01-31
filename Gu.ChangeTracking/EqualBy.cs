namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;

    public static class EqualBy
    {
        public static bool FieldValues<T>(T x, T y, params string[] excludedFields)
        {
            return FieldValues(x, y, Constants.DefaultFieldBindingFlags, excludedFields);
        }

        public static bool FieldValues<T>(T x, T y, BindingFlags bindingFlags, params string[] excludedFields)
        {
            Ensure.NotNull(x, nameof(x));
            Ensure.NotNull(y, nameof(y));
            Ensure.NotSame(x, y, nameof(x), nameof(y));
            Ensure.SameType(x, y, nameof(x), nameof(y));
            Ensure.NotIs<IEnumerable>(x, nameof(x));
            var fieldInfos = x.GetType().GetFields(bindingFlags);
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

                if (!IsEquatable(fieldInfo.FieldType))
                {
                    var message = $"Copy does not support comparing the field {fieldInfo.Name} of type {fieldInfo.FieldType}";
                    throw new NotSupportedException(message);
                }

                var xv = fieldInfo.GetValue(x);
                var yv = fieldInfo.GetValue(y);
                if (!Equals(xv, yv))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool PropertyValues<T>(T x, T y, params string[] excludedProperties)
        {
            return PropertyValues(x, y, Constants.DefaultFieldBindingFlags, excludedProperties);
        }

        public static bool PropertyValues<T>(T x, T y, BindingFlags bindingFlags, params string[] excludedProperties)
        {
            Ensure.NotNull(x, nameof(x));
            Ensure.NotNull(y, nameof(y));
            Ensure.NotSame(x, y, nameof(x), nameof(y));
            Ensure.SameType(x, y, nameof(x), nameof(y));
            Ensure.NotIs<IEnumerable>(x, nameof(x));
            var propertyInfos = x.GetType().GetProperties(bindingFlags);
            foreach (var propertyInfo in propertyInfos)
            {
                if (excludedProperties?.Contains(propertyInfo.Name) == true)
                {
                    continue;
                }

                if (!IsEquatable(propertyInfo.PropertyType))
                {
                    var message = $"Copy does not support comparing the property {propertyInfo.Name} of type {propertyInfo.PropertyType}";
                    throw new NotSupportedException(message);
                }

                var xv = propertyInfo.GetValue(x);
                var yv = propertyInfo.GetValue(y);
                if (!Equals(xv, yv))
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool IsEquatable(Type type)
        {
            if (type == typeof(string))
            {
                return true;
            }

            if (type.IsEnum)
            {
                return true;
            }

            if (type.IsNullable())
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                return IsEquatable(underlyingType);
            }

            return type.IsValueType && type.IsEquatable();
        }
    }
}