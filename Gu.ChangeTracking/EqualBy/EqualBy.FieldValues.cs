namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;

    public static partial class EqualBy
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
            return FieldValuesCore(x, y, bindingFlags, referenceHandling);
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

        private static bool FieldValuesCore<T>(T x, T y, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
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
                var xlist = x as IList;
                var ylist = y as IList;
                if (xlist != null && ylist != null)
                {
                    if (xlist.Count != ylist.Count)
                    {
                        return false;
                    }

                    for (int i = 0; i < xlist.Count; i++)
                    {
                        var xv = xlist[i];
                        var yv = ylist[i];

                        if (!FieldValueEquals<T>(xv, yv, bindingFlags, referenceHandling))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    // using ensure to throw
                    Ensure.NotIs<IEnumerable>(x, nameof(x));
                }
            }

            var fieldInfos = x.GetType().GetFields(bindingFlags);
            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.IsEventField())
                {
                    continue;
                }

                var xv = fieldInfo.GetValue(x);
                var yv = fieldInfo.GetValue(y);

                if (!FieldValueEquals<T>(xv, yv, bindingFlags, referenceHandling))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool FieldValueEquals<T>(object x, object y, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
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
                switch (referenceHandling)
                {
                    case ReferenceHandling.Reference:
                        if (ReferenceEquals(x, y))
                        {
                            return true;
                        }

                        return false;
                    case ReferenceHandling.Structural:
                        if (FieldValuesCore(x, y, bindingFlags, referenceHandling))
                        {
                            return true;
                        }

                        return false;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(referenceHandling), referenceHandling, null);
                }
            }

            return true;
        }
    }
}