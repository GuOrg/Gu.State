namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reflection;

    public static partial class DirtyTracker
    {
        public static void Verify<T>(params string[] ignoreProperties)
            where T : class, INotifyPropertyChanged
        {
            Verify<T>(Constants.DefaultPropertyBindingFlags, ignoreProperties);
        }

        /// <summary>
        /// Check if <typeparamref name="T"/> can be tracked
        /// </summary>
        public static void Verify<T>(BindingFlags bindingFlags, params string[] ignoreProperties)
                        where T : class, INotifyPropertyChanged
        {
            var settings = DirtyTrackerSettings.Create(typeof(T), ignoreProperties, bindingFlags, ReferenceHandling.Throw);
            Verify<T>(settings);
        }

        public static void Verify<T>(DirtyTrackerSettings settings)
                        where T : class, INotifyPropertyChanged
        {
            Verify(typeof(T), settings);
        }

        public static void Verify(Type type, DirtyTrackerSettings settings)
        {
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                if (settings.ReferenceHandling == ReferenceHandling.Throw || 
                    (settings.ReferenceHandling != ReferenceHandling.Throw
                        && !typeof(INotifyCollectionChanged).IsAssignableFrom(type)))
                {
                    throw new NotSupportedException(
                        "Not supporting IEnumerable unless ReferenceHandling is specified and the collection is INotifyCollectionChanged");
                }
            }

            foreach (var propertyInfo in type.GetProperties(settings.BindingFlags))
            {
                if (settings.IsIgnoringProperty(propertyInfo))
                {
                    continue;
                }

                if (!propertyInfo.PropertyType.IsImmutable() && settings.ReferenceHandling == ReferenceHandling.Throw)
                {
                    var message =
                        $"Only equatable properties are supported without specifying {typeof(ReferenceHandling).Name}\r\n"
                        + $"Property {type.Name}.{propertyInfo.Name} is not IEquatable<{propertyInfo.PropertyType.Name}>.\r\n"
                        + "Use the overload DirtyTracker.Track(x, y, ReferenceHandling) if you want to track a graph";
                    throw new NotSupportedException(message);
                }
            }
        }
    }
}
