namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    public partial class ChangeTracker
    {
        public static void VerifyCanTrack<T>(IIgnoringProperties settings)
        {
            VerifyCanTrack(typeof(T), settings);
        }

        public static void VerifyCanTrack(Type type, IIgnoringProperties settings)
        {
            Verify.IsTrackableType(type, settings);
        }

        /// <summary>
        /// This class is used for failing fast and throwing with nice exception messages.
        /// </summary>
        private static class Verify
        {
            private static readonly ConditionalWeakTable<IIgnoringProperties, ConcurrentSet<Type>> ValidTypesCache = new ConditionalWeakTable<IIgnoringProperties, ConcurrentSet<Type>>();

            internal static void IsTrackableType(Type type, ChangeTracker tracker)
            {
                if (ValidTypesCache.GetOrCreateValue(tracker.Settings).Contains(type))
                {
                    return;
                }

                IsTrackableType(type, tracker.Path, tracker.Settings);
            }

            internal static void IsTrackableType(Type type, IIgnoringProperties settings)
            {
                if (ValidTypesCache.GetOrCreateValue(settings).Contains(type))
                {
                    return;
                }

                IsTrackableType(type, new PropertyPath(type), settings);
            }

            internal static void IsTrackablePropertyValue(Type propertyValueType, PropertyInfo propertyInfo, ChangeTracker tracker)
            {
                if (ValidTypesCache.GetOrCreateValue(tracker.Settings).Contains(propertyValueType))
                {
                    return;
                }

                var path = tracker.Path.WithProperty(propertyInfo);
                IsTrackableType(propertyValueType, path, tracker.Settings);
            }

            internal static void IsTrackableItemValue(Type itemType, int? index, ChangeTracker tracker)
            {
                if (ValidTypesCache.GetOrCreateValue(tracker.Settings).Contains(itemType))
                {
                    return;
                }

                var path = tracker.Path.WithIndex(index);
                IsTrackableType(itemType, path, tracker.Settings);
            }

            private static void IsTrackableType(Type type, PropertyPath path, IIgnoringProperties settings)
            {
                var checkedTypes = ValidTypesCache.GetOrCreateValue(settings);
                if (checkedTypes.Contains(type))
                {
                    return;
                }

                CheckProperties(type, path, settings);
                CheckItemType(type, path, settings);
                checkedTypes.Add(type);
            }

            private static void IsTrackableIfEnumerable(Type type, PropertyPath propertyPath)
            {
                if (!typeof(IEnumerable).IsAssignableFrom(type))
                {
                    return;
                }

                if (!typeof(INotifyCollectionChanged).IsAssignableFrom(type) || !typeof(INotifyCollectionChanged).IsAssignableFrom(type))
                {
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendCreateFailed<ChangeTracker>(propertyPath)
                                  .AppendSolveTheProblemBy()
                                  .AppendSuggestionsForEnumerableLines(type)
                                  .AppendSuggestImmutableType(propertyPath)
                                  .AppendSuggestChangeTrackerSettings(type, propertyPath);

                    var message = messageBuilder.ToString();
                    throw new NotSupportedException(message);
                }
            }

            private static void IsPropertyChanged(Type type, PropertyPath propertyPath)
            {
                if (!typeof(INotifyPropertyChanged).IsAssignableFrom(type))
                {
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendCreateFailed<ChangeTracker>(propertyPath)
                                  .AppendSolveTheProblemBy()
                                  .AppendSuggestImplement<INotifyPropertyChanged>(type)
                                  .AppendSuggestImmutableType(propertyPath)
                                  .AppendSuggestChangeTrackerSettings(type, propertyPath);

                    var message = messageBuilder.ToString();
                    throw new NotSupportedException(message);
                }
            }

            private static void CheckProperties(Type type, PropertyPath path, IIgnoringProperties settings)
            {
                var properties = PropertiesChangeTrackers.GetTrackProperties(type, settings)
                                         .ToArray();
                foreach (var propertyInfo in properties)
                {
                    if (path.Path.OfType<PropertyItem>().Any(p => p.Property.PropertyType == type))
                    {
                        // stopping recursion if a type has self itemType as property
                        continue;
                    }

                    var propertyPath = path.WithProperty(propertyInfo);
                    if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        IsTrackableIfEnumerable(propertyInfo.PropertyType, propertyPath);
                    }
                    else if (!settings.IsIgnoringProperty(propertyInfo))
                    {
                        IsPropertyChanged(propertyInfo.PropertyType, propertyPath);
                    }

                    IsTrackableType(propertyInfo.PropertyType, propertyPath, settings);
                }
            }

            private static void CheckItemType(Type type, PropertyPath path, IIgnoringProperties settings)
            {
                if (!typeof(IEnumerable).IsAssignableFrom(type))
                {
                    return;
                }

                IsTrackableIfEnumerable(type, path);
                var itemType = type.GetItemType();
                var propertyPath = path.WithIndex(null);
                IsTrackableType(itemType, propertyPath, settings);
            }
        }
    }
}
