namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static partial class Copy
    {
        public static void FieldValues<T>(T source, T target, ReferenceHandling referenceHandling)
    where T : class
        {
            FieldValues(source, target, Constants.DefaultFieldBindingFlags, referenceHandling);
        }

        public static void FieldValues<T>(T source, T target, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            where T : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));
            Ensure.SameType(source, target);
            var sourceList = source as IList;
            var targetList = target as IList;
            if (sourceList != null && targetList != null)
            {
                SyncLists(sourceList, targetList, FieldValues, referenceHandling);
                return;
            }

            Ensure.NotIs<IEnumerable>(source, nameof(source));

            var fieldInfos = source.GetType().GetFields(bindingFlags);
            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.IsEventField())
                {
                    continue;
                }

                if (!IsCopyableType(fieldInfo.FieldType) && !fieldInfo.IsInitOnly)
                {
                    var sourceValue = fieldInfo.GetValue(source);
                    if (sourceValue == null)
                    {
                        fieldInfo.SetValue(target, null);
                        continue;
                    }

                    switch (referenceHandling)
                    {
                        case ReferenceHandling.Reference:
                            fieldInfo.SetValue(target, sourceValue);
                            continue;
                        case ReferenceHandling.Structural:
                            var targetValue = fieldInfo.GetValue(target);
                            if (targetValue != null)
                            {
                                FieldValues(sourceValue, targetValue, referenceHandling);
                                continue;
                            }

                            targetValue = Activator.CreateInstance(sourceValue.GetType(), true);
                            FieldValues(sourceValue, targetValue, referenceHandling);
                            fieldInfo.SetValue(target, targetValue);
                            continue;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(referenceHandling), referenceHandling, null);
                    }
                }
                else
                {
                    FieldValue(source, target, fieldInfo);
                }
            }
        }

        /// <summary>
        /// Copies field values from source to target.
        /// Only valur types and string are allowed.
        /// </summary>
        public static void FieldValues<T>(T source, T target, params string[] excludedFields) where T : class
        {
            FieldValues(source, target, Constants.DefaultFieldBindingFlags, excludedFields);
        }

        /// <summary>
        /// Copies field values from source to target.
        /// Only valur types and string are allowed.
        /// </summary>
        public static void FieldValues<T>(T source, T target, BindingFlags bindingFlags, params string[] excludedFields) where T : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));
            Ensure.SameType(source, target);
            Ensure.NotIs<IEnumerable>(source, nameof(source));

            var fieldInfos = source.GetType().GetFields(bindingFlags);
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

                FieldValue(source, target, fieldInfo);
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

            var fieldInfos = typeof(T).GetFields(bindingFlags).Where(f => ignoreFields?.All(pn => pn != f.Name) == true && !f.IsEventField()).ToArray();

            var illegalTypes = fieldInfos.Where(p => !IsCopyableType(p.FieldType)).ToArray();

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

        private static void FieldValue(object source, object target, FieldInfo fieldInfo)
        {
            var sourceValue = fieldInfo.GetValue(source);
            if (fieldInfo.IsInitOnly)
            {
                var targetValue = fieldInfo.GetValue(target);
                if (!Equals(sourceValue, targetValue))
                {
                    var message = $"Field {source.GetType().Name}.{fieldInfo.Name} differs but cannot be updated because it is readonly.\r\n" + $"Provide {typeof(Copy).Name}.{nameof(FieldValues)}(x, y, nameof({source.GetType().Name}.{fieldInfo.Name}))";
                    throw new InvalidOperationException(message);
                }
            }
            else
            {
                fieldInfo.SetValue(target, sourceValue);
            }
        }
    }
}