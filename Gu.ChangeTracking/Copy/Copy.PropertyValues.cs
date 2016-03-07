namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static partial class Copy
    {
        /// <summary>
        /// Copies property values from source to target.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="source" /> and <paramref name="target" /></typeparam>
        /// <param name="source">The instance to copy property values from</param>
        /// <param name="target">The instance to copy property values to</param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        public static void PropertyValues<T>(
            T source,
            T target,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw)
            where T : class
        {
            var settings = PropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
            PropertyValues(source, target, settings);
        }

        /// <summary>
        /// Copies property values from source to target.
        /// </summary>
        /// <typeparam name="T">The type to to copy</typeparam>
        /// <param name="source">The instance to copy property values from</param>
        /// <param name="target">The instance to copy property values to</param>
        /// <param name="settings">Contains configuration for how copy will be performed</param>
        public static void PropertyValues<T>(T source, T target, PropertiesSettings settings)
            where T : class
        {
            Verify.CanCopyRoot(typeof(T));
            var type = source?.GetType() ?? target?.GetType() ?? typeof(T);
            VerifyCanCopyPropertyValues(type, settings);
            if (settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops)
            {
                var referencePairs = new ReferencePairCollection();
                CopyPropertiesValues(source, target, settings, referencePairs);
            }
            else
            {
                CopyPropertiesValues(source, target, settings, null);
            }
        }

        private static void CopyPropertiesValues<T>(T source, T target, PropertiesSettings settings, ReferencePairCollection referencePairs)
            where T : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));
            Ensure.SameType(source, target);
            Verify.CanCopyPropertyValues(source, target, settings);
            if (referencePairs?.Contains(source, target) == true)
            {
                return;
            }

            referencePairs?.Add(source, target);

            CopyCollectionItems(source, target, CopyPropertiesValues, settings, referencePairs);
            var propertyInfos = source.GetType().GetProperties(settings.BindingFlags);
            CopyWritablePropertiesValues(source, target, propertyInfos, settings, referencePairs);
            VerifyReadonlyPropertiesAreEqual(source, target, propertyInfos, settings, referencePairs);
        }

        private static void CopyWritablePropertiesValues(
            object source,
            object target,
            IReadOnlyList<PropertyInfo> propertyInfos,
            PropertiesSettings settings,
            ReferencePairCollection referencePairs)
        {
            foreach (var propertyInfo in propertyInfos)
            {
                if (settings.IsIgnoringProperty(propertyInfo))
                {
                    continue;
                }

                var specialCopyProperty = settings.GetSpecialCopyProperty(propertyInfo);
                if (specialCopyProperty != null)
                {
                    specialCopyProperty.CopyValue(source, target);
                    continue;
                }

                if (propertyInfo.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                var sv = propertyInfo.GetValue(source);
                if (!propertyInfo.CanWrite)
                {
                    if (propertyInfo.PropertyType.IsImmutable())
                    {
                        continue;
                    }

                    var tv = propertyInfo.GetValue(target);
                    if (sv != null && tv != null)
                    {
                        CopyPropertiesValues(sv, tv, settings, referencePairs);
                    }

                    continue;
                }

                if (IsCopyableType(propertyInfo.PropertyType))
                {
                    propertyInfo.SetValue(target, sv);
                    continue;
                }

                switch (settings.ReferenceHandling)
                {
                    case ReferenceHandling.References:
                        {
                            var value = propertyInfo.GetValue(source);
                            propertyInfo.SetValue(target, value);
                            break;
                        }

                    case ReferenceHandling.Structural:
                    case ReferenceHandling.StructuralWithReferenceLoops:
                        var sourceValue = propertyInfo.GetValue(source);
                        if (sourceValue == null)
                        {
                            propertyInfo.SetValue(target, null);
                            continue;
                        }

                        var targetValue = propertyInfo.GetValue(target);
                        if (targetValue == null)
                        {
                            targetValue = CreateInstance(sourceValue, propertyInfo, settings);
                            propertyInfo.SetValue(target, targetValue, null);
                        }

                        CopyPropertiesValues(sourceValue, targetValue, settings, referencePairs);
                        continue;
                    case ReferenceHandling.Throw:
                        throw ChangeTracking.Throw.ShouldNeverGetHereException();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(settings.ReferenceHandling), settings.ReferenceHandling, null);
                }
            }
        }

        private static void VerifyReadonlyPropertiesAreEqual(
            object source,
            object target,
            IReadOnlyList<PropertyInfo> propertyInfos,
            PropertiesSettings settings,
            ReferencePairCollection referencePairs)
        {
            foreach (var propertyInfo in propertyInfos)
            {
                if (settings.IsIgnoringProperty(propertyInfo))
                {
                    continue;
                }

                if (propertyInfo.CanWrite)
                {
                    continue;
                }

                var sv = propertyInfo.GetValue(source);
                var tv = propertyInfo.GetValue(target);
                if (sv == null && tv == null)
                {
                    continue;
                }

                if (sv?.GetType().IsImmutable() == false)
                {
                    CopyPropertiesValues(sv, tv, settings, referencePairs);
                }

                if (!EqualBy.PropertyValues(sv, tv, settings))
                {
                    Throw.ReadonlyMemberDiffers(new SourceAndTargetValue(source, sv, target, tv), propertyInfo, settings);
                }
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
                    var message = "There is a bug in the library as it:\r\n"
                                  + $"Tried to copy the value of the readonly property {source.GetType().PrettyName()}{propertyInfo.Name}.";
                    throw new InvalidOperationException(message);
                }
            }
        }
    }
}
