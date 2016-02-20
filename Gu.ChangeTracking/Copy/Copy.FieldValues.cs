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
            var settings = CopyFieldsSettings.GetOrCreate(Constants.DefaultFieldBindingFlags, referenceHandling);
            FieldValues(source, target, settings);
        }

        public static void FieldValues<T>(T source, T target, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            where T : class
        {
            var settings = CopyFieldsSettings.GetOrCreate(bindingFlags, referenceHandling);
            FieldValues(source, target, settings);
        }

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
            var settings = new CopyFieldsSettings(source?.GetType().GetIgnoreFields(bindingFlags, excludedFields), bindingFlags, ReferenceHandling.Throw);
            FieldValues(source, target, settings);
        }

        public static void FieldValues<T>(T source, T target, CopyFieldsSettings settings)
            where T : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));
            Ensure.SameType(source, target);
            var sourceList = source as IList;
            var targetList = target as IList;
            if (sourceList != null && targetList != null && settings.ReferenceHandling != ReferenceHandling.Throw)
            {
                SyncLists(sourceList, targetList, FieldValues, settings);
                return;
            }

            Ensure.NotIs<IEnumerable>(source, nameof(source));

            var fieldInfos = source.GetType().GetFields(settings.BindingFlags);
            foreach (var fieldInfo in fieldInfos)
            {
                if (settings.IsIgnoringField(fieldInfo))
                {
                    continue;
                }

                if (!IsCopyableType(fieldInfo.FieldType) && !fieldInfo.IsInitOnly)
                {
                    var sourceValue = fieldInfo.GetValue(source);
                    switch (settings.ReferenceHandling)
                    {
                        case ReferenceHandling.Reference:
                            fieldInfo.SetValue(target, sourceValue);
                            continue;
                        case ReferenceHandling.Structural:
                            if (sourceValue == null)
                            {
                                fieldInfo.SetValue(target, null);
                                continue;
                            }

                            var targetValue = fieldInfo.GetValue(target);
                            if (targetValue != null)
                            {
                                FieldValues(sourceValue, targetValue, settings);
                                continue;
                            }

                            targetValue = Activator.CreateInstance(sourceValue.GetType(), true);
                            FieldValues(sourceValue, targetValue, settings);
                            fieldInfo.SetValue(target, targetValue);
                            continue;
                        case ReferenceHandling.Throw:
                            var message = "Only fields with types struct or string are supported without specifying ReferenceHandling\r\n" +
                                         $"Field {source.GetType().Name}.{fieldInfo.Name} is a reference type ({fieldInfo.FieldType.Name}).\r\n" +
                                          "Use the overload Copy.FieldValues(source, target, ReferenceHandling) if you want to copy a graph";
                            throw new NotSupportedException(message);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(settings.ReferenceHandling), settings.ReferenceHandling, null);
                    }
                }
                else
                {
                    FieldValue(source, target, fieldInfo);
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