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
        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be tracked.
        /// This method will throw an exception if synchronization cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        public static void VerifyCanTrack<T>()
        {
            VerifyCanTrack<T>(PropertiesSettings.GetOrCreate());
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be tracked.
        /// This method will throw an exception if synchronization cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <param name="settings">Contains configuration for how tracking will be performed</param>
        public static void VerifyCanTrack<T>(PropertiesSettings settings)
        {
            VerifyCanTrack(typeof(T), settings);
        }

        /// <summary>
        /// Check if the properties of <paramref name="type"/> can be tracked.
        /// This method will throw an exception if trackling cannot be performed for <paramref name="type"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="settings">Contains configuration for how tracking will be performed</param>
        public static void VerifyCanTrack(Type type, PropertiesSettings settings)
        {
            Verify.IsTrackableType(type, settings);
        }

        /// <summary>
        /// This class is used for failing fast and throwing with nice exception messages.
        /// </summary>
        private static class Verify
        {
            private static readonly ConditionalWeakTable<PropertiesSettings, ConcurrentSet<Type>> ValidTypesCache = new ConditionalWeakTable<PropertiesSettings, ConcurrentSet<Type>>();

            internal static void IsTrackableType(Type type, ChangeTracker tracker)
            {
                if (ValidTypesCache.GetOrCreateValue(tracker.Settings).Contains(type))
                {
                    return;
                }

                IsTrackableType(type, tracker.Path, tracker.Settings);
            }

            internal static void IsTrackableType(Type type, PropertiesSettings settings)
            {
                if (ValidTypesCache.GetOrCreateValue(settings).Contains(type))
                {
                    return;
                }

                IsTrackableType(type, new MemberPath(type), settings);
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

            private static void IsTrackableType(Type type, MemberPath path, PropertiesSettings settings)
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

            private static void IsTrackableIfEnumerable(Type type, MemberPath memberPath)
            {
                if (!typeof(IEnumerable).IsAssignableFrom(type))
                {
                    return;
                }

                if (!typeof(INotifyCollectionChanged).IsAssignableFrom(type) || !typeof(INotifyCollectionChanged).IsAssignableFrom(type))
                {
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendCreateFailed<ChangeTracker>(memberPath)
                                  .AppendSolveTheProblemBy()
                                  .AppendSuggestionsForEnumerableLines(type)
                                  .AppendSuggestImmutableType(memberPath)
                                  .AppendSuggestChangeTrackerSettings(type, memberPath);

                    var message = messageBuilder.ToString();
                    throw new NotSupportedException(message);
                }
            }

            private static void IsPropertyChanged(Type type, MemberPath memberPath)
            {
                if (!typeof(INotifyPropertyChanged).IsAssignableFrom(type))
                {
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendCreateFailed<ChangeTracker>(memberPath)
                                  .AppendSolveTheProblemBy()
                                  .AppendSuggestImplement<INotifyPropertyChanged>(type)
                                  .AppendSuggestImmutableType(memberPath)
                                  .AppendSuggestChangeTrackerSettings(type, memberPath);

                    var message = messageBuilder.ToString();
                    throw new NotSupportedException(message);
                }
            }

            private static void CheckProperties(Type type, MemberPath path, PropertiesSettings settings)
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

            private static void CheckItemType(Type type, MemberPath path, PropertiesSettings settings)
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
