namespace Gu.ChangeTracking
{
    using System;
    using System.Reflection;

    public static partial class Copy
    {
        /// <summary>
        /// Copies field values from source to target.
        /// Event fields are excluded
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for</typeparam>
        /// <param name="source">The instance to copy field values from</param>
        /// <param name="target">The instance to copy field values to</param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used field values for sub fields are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        public static void FieldValues<T>(
            T source,
            T target,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw)
            where T : class
        {
            var settings = FieldsSettings.GetOrCreate(bindingFlags, referenceHandling);
            FieldValues(source, target, settings);
        }

        /// <summary>
        /// Copies field values from source to target.
        /// Event fields are excluded
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for</typeparam>
        /// <param name="source">The instance to copy field values from</param>
        /// <param name="target">The instance to copy field values to</param>
        /// <param name="settings">Contains configuration for how to copy</param>
        public static void FieldValues<T>(T source, T target, FieldsSettings settings)
            where T : class
        {
            Verify.CanCopyRoot(typeof(T));
            var type = source?.GetType() ?? target?.GetType() ?? typeof(T);
            VerifyCanCopyFieldValues(type, settings);
            if (settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops)
            {
                var referencePairs = new ReferencePairCollection();
                CopyFieldsValues(source, target, settings, referencePairs);
            }
            else
            {
                CopyFieldsValues(source, target, settings, null);
            }
        }

        private static void CopyFieldsValues<T>(T source, T target, FieldsSettings settings, ReferencePairCollection referencePairs)
            where T : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));
            Ensure.SameType(source, target);
            Verify.CanCopyFieldValues(source, target, settings);
            if (referencePairs?.Contains(source, target) == true)
            {
                return;
            }

            referencePairs?.Add(source, target);

            CopyCollectionItems(source, target, CopyFieldsValues, settings, referencePairs);

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
                        CopyFieldsValues(sv, tv, settings, referencePairs);
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
                    case ReferenceHandling.StructuralWithReferenceLoops:
                        if (sv == null)
                        {
                            fieldInfo.SetValue(target, null);
                            continue;
                        }

                        var targetValue = fieldInfo.GetValue(target);
                        if (targetValue != null)
                        {
                            CopyFieldsValues(sv, targetValue, settings, referencePairs);
                            continue;
                        }

                        targetValue = CreateInstance(sv, fieldInfo, settings);
                        CopyFieldsValues(sv, targetValue, settings, referencePairs);
                        fieldInfo.SetValue(target, targetValue);
                        continue;
                    case ReferenceHandling.Throw:
                        throw ChangeTracking.Throw.ShouldNeverGetHereException();
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
                    var message = $"Field {source.GetType().Name}.{fieldInfo.Name} differs but cannot be updated because it is readonly.\r\n" + $"Provide {typeof(Copy).Name}.{nameof(CopyFieldsValues)}(x, y, nameof({source.GetType().Name}.{fieldInfo.Name}))";
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