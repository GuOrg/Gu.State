namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static class Synchronizer
    {
        /// <summary>
        /// Copies values for properties not marked with [XmlIgnore] from source to destination.
        /// Traverses nested properties recursively
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static void SynchronizeFields<T>(T source, T destination)
        {
            Ensure.NotNull(source, "source");
            Ensure.NotNull(destination, "destination");
            if (source.GetType() != destination.GetType())
            {
                throw new ArgumentException("Source and destination must be the same type");
            }
            if (source is IEnumerable)
            {
                throw new InvalidOperationException("Not supporting IEnumerable");
            }

            var fieldInfos = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var fieldInfo in fieldInfos)
            {
                var value = fieldInfo.GetValue(source);
                fieldInfo.SetValue(destination, value);
            }
        }

        public static void SynchronizeProperties<T>(T source, T destination, params string[] ignoreProperties)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(destination, "destination");
            if (source.GetType() != destination.GetType())
            {
                throw new ArgumentException("Source and destination must be the same type");
            }

            if (source is IEnumerable)
            {
                throw new InvalidOperationException("Not supporting lists");
            }

            var propertyInfos = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var propertyInfo in propertyInfos)
            {
                if (ignoreProperties != null && ignoreProperties.Any(x => propertyInfo.Name == x))
                {
                    continue;
                }

                var value = propertyInfo.GetValue(source);
                propertyInfo.SetValue(destination, value, null);
            }
        }

        public static IDisposable CreatePropertySynchronizer<T>(T source, T destination, params string[] ignoreProperties)
            where T : INotifyPropertyChanged
        {
            SynchronizeProperties(source, destination, ignoreProperties);
            return new PropertySynchronizer<T>(source, destination, ignoreProperties);
        }

        /// <summary>
        /// This is just a quick and dirty to fail fast.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static bool VerifySynchronizeFields<T>(T source, T destination)
        {
            try
            {
                SynchronizeFields(source, destination);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// This is just a quick and dirty to fail fast.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static void VerifyProperties<T>(params string[] ignoreProperties)
        {
            var propertyInfos = typeof(T).GetProperties()
                .Where(p => ignoreProperties?.All(pn => pn != p.Name) == true)
                .ToArray();

            var missingSetters = propertyInfos.Where(p => p.SetMethod == null)
                .ToArray();
            var illegalTypes = propertyInfos.Where(p => !(p.PropertyType.IsValueType || p.PropertyType == typeof(string)))
                .ToArray();
            if (missingSetters.Any() || illegalTypes.Any())
            {
                var stringBuilder = new StringBuilder();
                if (missingSetters.Any())
                {
                    stringBuilder.AppendLine("Missing setters:");
                    foreach (var prop in missingSetters)
                    {
                        stringBuilder.AppendLine($"The property {prop.Name} does not have a setter");
                    }
                }

                if (illegalTypes.Any())
                {
                    stringBuilder.AppendLine("Illegal types:");
                    foreach (var prop in illegalTypes)
                    {
                        stringBuilder.AppendLine($"The property {prop.Name} is not of a supported type. Expected valuetype of string but was {prop.PropertyType}");
                    }
                }
                var message = stringBuilder.ToString();
                throw new InvalidOperationException(message);
            }
        }

        private class PropertySynchronizer<T> : IDisposable
            where T : INotifyPropertyChanged
        {
            private readonly T _source;
            private readonly T _target;
            private readonly IReadOnlyList<string> _ignoreProperties;
            private readonly IDisposable _subscription;

            public PropertySynchronizer(T source, T target, IReadOnlyList<string> ignoreProperties)
            {
                this._source = source;
                this._target = target;
                this._ignoreProperties = ignoreProperties;
                this._subscription = this._source.ObservePropertyChangedSlim()
                    .Subscribe(x => SyncProperty(x.PropertyName));
            }

            public void Dispose()
            {
                this._subscription.Dispose();
            }

            private void SyncProperty(string propertyName)
            {
                if (this._ignoreProperties?.Contains(propertyName) == true)
                {
                    return;
                }

                var propertyInfo = this._source.GetType().GetProperty(propertyName);
                var value = propertyInfo.GetValue(this._source);
                propertyInfo.SetValue(this._target, value);
            }
        }
    }
}