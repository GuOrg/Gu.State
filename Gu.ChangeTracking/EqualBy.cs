namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;

    public static class EqualBy
    {
        public static bool FieldValues<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return FieldValues(x, y, Constants.DefaultFieldBindingFlags, referenceHandling);
        }

        public static bool FieldValues<T>(T x, T y, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            Ensure.SameType(x, y, nameof(x), nameof(y));
            Ensure.NotIs<IEnumerable>(x, nameof(x));
            var fieldInfos = x.GetType().GetFields(bindingFlags);
            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.IsEventField())
                {
                    continue;
                }

                if (!IsEquatable(fieldInfo.FieldType))
                {
                    switch (referenceHandling)
                    {
                        case ReferenceHandling.Reference:
                            if (ReferenceEquals(fieldInfo.GetValue(x), fieldInfo.GetValue(y)))
                            {
                                continue;
                            }

                            return false;
                        case ReferenceHandling.Structural:
                            var xValue = fieldInfo.GetValue(x);
                            var yValue = fieldInfo.GetValue(y);
                            if (FieldValues(xValue, yValue, referenceHandling))
                            {
                                continue;
                            }

                            return false;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(referenceHandling), referenceHandling, null);
                    }
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

        public static bool FieldValues<T>(T x, T y, params string[] excludedFields)
        {
            return FieldValues(x, y, Constants.DefaultFieldBindingFlags, excludedFields);
        }

        public static bool FieldValues<T>(T x, T y, BindingFlags bindingFlags, params string[] excludedFields)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

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

        public static bool PropertyValues<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return PropertyValues(x, y, Constants.DefaultPropertyBindingFlags, referenceHandling);
        }

        public static bool PropertyValues<T>(T x, T y, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            Ensure.SameType(x, y, nameof(x), nameof(y));
            Ensure.NotIs<IEnumerable>(x, nameof(x));
            var propertyInfos = x.GetType().GetProperties(bindingFlags);
            foreach (var propertyInfo in propertyInfos)
            {
                if (!IsEquatable(propertyInfo.PropertyType))
                {
                    switch (referenceHandling)
                    {
                        case ReferenceHandling.Reference:
                            if (ReferenceEquals(propertyInfo.GetValue(x), propertyInfo.GetValue(y)))
                            {
                                continue;
                            }

                            return false;
                        case ReferenceHandling.Structural:
                            var xValue = propertyInfo.GetValue(x);
                            var yValue = propertyInfo.GetValue(y);
                            if (PropertyValues(xValue, yValue))
                            {
                                continue;
                            }

                            return false;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(referenceHandling), referenceHandling, null);
                    }
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

        public static bool PropertyValues<T>(T x, T y, params string[] excludedProperties)
        {
            return PropertyValues(x, y, Constants.DefaultPropertyBindingFlags, excludedProperties);
        }

        public static bool PropertyValues<T>(T x, T y, BindingFlags bindingFlags, params string[] excludedProperties)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

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
            if (type == typeof (string))
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