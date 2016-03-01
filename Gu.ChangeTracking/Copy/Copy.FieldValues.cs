namespace Gu.ChangeTracking
{
    using System;
    using System.Reflection;

    public static partial class Copy
    {
        public static void FieldValues<T>(T source, T target, BindingFlags bindingFlags)
            where T : class
        {
            var settings = CopyFieldsSettings.GetOrCreate(bindingFlags);
            FieldValues(source, target, settings);
        }

        public static void FieldValues<T>(T source, T target, ReferenceHandling referenceHandling)
            where T : class
        {
            var settings = CopyFieldsSettings.GetOrCreate(referenceHandling);
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
            var settings = CopyFieldsSettings.Create(source, target, bindingFlags, ReferenceHandling.Throw, excludedFields);
            FieldValues(source, target, settings);
        }

        public static void FieldValues<T>(T source, T target, CopyFieldsSettings settings)
            where T : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));
            Ensure.SameType(source, target);
            Verify.Indexers(source.GetType(), settings);
            CopyCollectionItems(source, target, FieldValues, settings);

            var fieldInfos = source.GetType().GetFields(settings.BindingFlags);
            foreach (var fieldInfo in fieldInfos)
            {
                if (settings.IsIgnoringField(fieldInfo))
                {
                    continue;
                }

                var sv = fieldInfo.GetValue(source);
                if (ReferenceEquals(sv, source))
                {
                    continue;
                }

                if (fieldInfo.IsInitOnly)
                {
                    var tv = fieldInfo.GetValue(target);
                    if (sv == null && tv == null)
                    {
                        continue;
                    }

                    if (fieldInfo.FieldType.IsImmutable())
                    {
                        if (!EqualBy.FieldValues(sv, tv, settings))
                        {
                            Throw.ReadonlyMemberDiffers(new SourceAndTargetValue(source, sv, target, tv), fieldInfo, settings);
                        }

                        continue;
                    }

                    if (sv != null && tv != null)
                    {
                        FieldValues(sv, tv, settings);
                        continue;
                    }

                    Throw.ReadonlyMemberDiffers(new SourceAndTargetValue(source, sv, target, tv), fieldInfo, settings);
                }

                if (IsCopyableType(fieldInfo.FieldType) && !fieldInfo.IsInitOnly)
                {
                    FieldValue(source, target, fieldInfo);
                    continue;
                }

                switch (settings.ReferenceHandling)
                {
                    case ReferenceHandling.References:
                        fieldInfo.SetValue(target, sv);
                        continue;
                    case ReferenceHandling.Structural:
                        if (sv == null)
                        {
                            fieldInfo.SetValue(target, null);
                            continue;
                        }

                        var targetValue = fieldInfo.GetValue(target);
                        if (targetValue != null)
                        {
                            FieldValues(sv, targetValue, settings);
                            continue;
                        }

                        targetValue = CreateInstance<CopyFieldsSettings>(sv, fieldInfo);
                        FieldValues(sv, targetValue, settings);
                        fieldInfo.SetValue(target, targetValue);
                        continue;
                    case ReferenceHandling.Throw:
                        Throw.CannotCopyMember(source.GetType(), fieldInfo, settings);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(settings.ReferenceHandling), settings.ReferenceHandling, null);
                }
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