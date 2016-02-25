namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Reflection;

    public static partial class EqualBy
    {
        public static bool FieldValues<T>(T x, T y, BindingFlags bindingFlags)
        {
            var settings = EqualByFieldsSettings.GetOrCreate(bindingFlags, ReferenceHandling.Throw);
            return FieldValues(x, y, settings);
        }

        /// <summary>
        /// Compares x and y for equality using field values.
        /// If a type implements IList the items of the list are compared
        /// </summary>
        /// <param name="referenceHandling">
        /// Specifies how reference types are compared.
        /// Structural compares field values recursively.
        /// </param>
        public static bool FieldValues<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            var settings = EqualByFieldsSettings.GetOrCreate(Constants.DefaultFieldBindingFlags, referenceHandling);
            return FieldValues(x, y, settings);
        }

        /// <summary>
        /// Compares x and y for equality using field values.
        /// If a type implements IList the items of the list are compared
        /// </summary>
        /// <param name="referenceHandling">
        /// Specifies how reference types are compared.
        /// Structural compares field values recursively.
        /// </param>
        public static bool FieldValues<T>(T x, T y, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            var settings = EqualByFieldsSettings.GetOrCreate(bindingFlags, referenceHandling);
            return FieldValues(x, y, settings);
        }

        public static bool FieldValues<T>(T x, T y, params string[] excludedFields)
        {
            return FieldValues(x, y, Constants.DefaultFieldBindingFlags, excludedFields);
        }

        public static bool FieldValues<T>(T x, T y, BindingFlags bindingFlags, params string[] excludedFields)
        {
            return FieldValues(x, y, new EqualByFieldsSettings(x.GetType().GetIgnoreFields(bindingFlags, excludedFields), bindingFlags, ReferenceHandling.Throw));
        }

        public static bool FieldValues<T>(T x, T y, EqualByFieldsSettings settings)
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
            if (settings.ReferenceHandling == ReferenceHandling.Throw)
            {
                Ensure.NotIs<IEnumerable>(x, nameof(x));
            }

            return FieldValuesCore(x, y, settings);
        }

        private static bool FieldValuesCore(object x, object y, EqualByFieldsSettings settings)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            if (x is IEnumerable)
            {
                if (!EnumerableEquals(x, y, FieldItemEquals, settings))
                {
                    return false;
                }
            }

            var fieldInfos = x.GetType().GetFields(settings.BindingFlags);
            foreach (var fieldInfo in fieldInfos)
            {
                if (settings.IsIgnoringField(fieldInfo))
                {
                    continue;
                }

                var xv = fieldInfo.GetValue(x);
                var yv = fieldInfo.GetValue(y);

                if (!FieldValueEquals(xv, yv, fieldInfo, settings))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool FieldItemEquals(object x, object y, EqualByFieldsSettings settings)
        {
            return FieldValueEquals(x, y, null, settings);
        }

        private static bool FieldValueEquals(object x, object y, FieldInfo fieldInfo, EqualByFieldsSettings settings)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (IsEquatable(x.GetType()))
            {
                if (!Equals(x, y))
                {
                    return false;
                }
            }
            else
            {
                switch (settings.ReferenceHandling)
                {
                    case ReferenceHandling.Reference:
                        if (ReferenceEquals(x, y))
                        {
                            return true;
                        }

                        return false;
                    case ReferenceHandling.Structural:
                        if (FieldValuesCore(x, y, settings))
                        {
                            return true;
                        }

                        return false;
                    case ReferenceHandling.Throw:
                        var message = $"EqualBy does not support comparing the field {fieldInfo.Name} of type {fieldInfo.FieldType}";
                        throw new NotSupportedException(message);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(settings.ReferenceHandling), settings.ReferenceHandling, null);
                }
            }

            return true;
        }
    }
}