namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Reflection;

    public static partial class EqualBy
    {
        public static bool FieldValues<T>(T x, T y, BindingFlags bindingFlags)
        {
            var settings = EqualByFieldsSettings.GetOrCreate(bindingFlags);
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
            var settings = EqualByFieldsSettings.GetOrCreate(referenceHandling);
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
            var settings = EqualByFieldsSettings.Create(x, y, bindingFlags, ReferenceHandling.Throw, excludedFields);
            return FieldValues(x, y, settings);
        }

        public static bool FieldValues<T>(T x, T y, IEqualByFieldsSettings settings)
        {
            Verify.FieldTypes(x, y, settings);
            Verify.Indexers(x?.GetType() ?? y?.GetType(), settings);
            Verify.Enumerable(x, y, settings);
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

            if (IsEquatable(x.GetType()))
            {
                return Equals(x, y);
            }

            if (settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops)
            {
                var referencePairs = new ReferencePairCollection();
                referencePairs.Add(x, y);
                return FieldsValuesEquals(x, y, settings, referencePairs);
            }
            else
            {
                return FieldsValuesEquals(x, y, settings, null);
            }
        }

        private static bool FieldsValuesEquals(object x, object y, IEqualByFieldsSettings settings, ReferencePairCollection referencePairs)
        {
            Verify.Indexers(x?.GetType() ?? y?.GetType(), settings);
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
                if (!EnumerableEquals(x, y, FieldItemEquals, settings, referencePairs))
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
                if (referencePairs?.Contains(xv, yv) == true)
                {
                    continue;
                }

                if (!FieldValueEquals(xv, yv, fieldInfo, settings, referencePairs))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool FieldItemEquals(object x, object y, IEqualByFieldsSettings settings, ReferencePairCollection referencePairs)
        {
            return FieldValueEquals(x, y, null, settings, referencePairs);
        }

        private static bool FieldValueEquals(object x, object y, FieldInfo fieldInfo, IEqualByFieldsSettings settings, ReferencePairCollection referencePairs)
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
                    case ReferenceHandling.References:
                        return ReferenceEquals(x, y);
                    case ReferenceHandling.Structural:
                    case ReferenceHandling.StructuralWithReferenceLoops:
                        return FieldsValuesEquals(x, y, settings, referencePairs);
                    case ReferenceHandling.Throw:
                        Throw.CannotCompareMember(x.GetType(), fieldInfo);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(settings.ReferenceHandling), settings.ReferenceHandling, null);
                }
            }

            return true;
        }
    }
}