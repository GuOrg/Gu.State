namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static class Synchronizer
    {
        private static readonly BindingFlags BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Copies field values from source to destination.
        /// Only valur types and string are allowed.
        /// </summary>
        public static void SynchronizeFields<T>(T source, T destination, params string[] ignoredFields)
            where T : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(destination, nameof(destination));
            Ensure.SameType(source, destination);
            Ensure.NotIs<IEnumerable>(source, nameof(source));

            var fieldInfos = typeof(T).GetFields(BindingFlags);
            foreach (var fieldInfo in fieldInfos)
            {
                if (ignoredFields?.Contains(fieldInfo.Name) == true)
                {
                    continue;
                }

                if (IsEventField(fieldInfo))
                {
                    continue;
                }

                if (!IsCopyableType(fieldInfo.FieldType))
                {
                    throw new InvalidOperationException();
                }

                var value = fieldInfo.GetValue(source);
                fieldInfo.SetValue(destination, value);
            }
        }

        /// <summary>
        /// Copies property values from source to destination.
        /// Only valur types and string are allowed.
        /// </summary>
        public static void SynchronizeProperties<T>(T source, T destination, params string[] ignoreProperties) where T : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(destination, nameof(destination));
            Ensure.SameType(source, destination);
            Ensure.NotIs<IEnumerable>(source, nameof(source));

            var propertyInfos = typeof(T).GetProperties(BindingFlags);
            foreach (var propertyInfo in propertyInfos)
            {
                if (ignoreProperties?.Contains(propertyInfo.Name) == true)
                {
                    continue;
                }

                if (!IsCopyableType(propertyInfo.PropertyType))
                {
                    throw new InvalidOperationException();
                }

                var value = propertyInfo.GetValue(source);
                propertyInfo.SetValue(destination, value, null);
            }
        }

        public static IDisposable CreatePropertySynchronizer<T>(T source, T destination, params string[] ignoreProperties)
            where T : class, INotifyPropertyChanged
        {
            SynchronizeProperties(source, destination, ignoreProperties);
            return new PropertySynchronizer<T>(source, destination, ignoreProperties);
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be synchronized.
        /// Use this to fail fast.
        /// </summary>
        public static void VerifyProperties<T>(params string[] ignoreProperties)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException("Not supporting IEnumerable");
            }

            var propertyInfos = typeof(T).GetProperties()
                .Where(p => ignoreProperties?.All(pn => pn != p.Name) == true)
                .ToArray();

            var missingSetters = propertyInfos.Where(p => p.SetMethod == null)
                .ToArray();
            var illegalTypes = propertyInfos.Where(p => !IsCopyableType(p.PropertyType))
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

        /// <summary>
        /// Check if the fields of <typeparamref name="T"/> can be synchronized.
        /// Use this to fail fast.
        /// </summary>
        public static void VerifyFields<T>(params string[] ignoreFields)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException("Not supporting IEnumerable");
            }

            var fieldInfos = typeof(T).GetFields(BindingFlags)
                .Where(f => ignoreFields?.All(pn => pn != f.Name) == true && !IsEventField(f))
                .ToArray();

            var illegalTypes = fieldInfos.Where(p => !IsCopyableType(p.FieldType))
                .ToArray();

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
                throw new InvalidOperationException(message);
            }
        }

        private static bool IsCopyableType(Type type)
        {
            return type.IsValueType || type == typeof(string);
        }

        private static bool IsEventField(FieldInfo field)
        {
            return typeof(MulticastDelegate).IsAssignableFrom(field.FieldType);
        }

        private class PropertySynchronizer<T> : IDisposable
            where T : class, INotifyPropertyChanged
        {
            private readonly T source;
            private readonly T target;
            private readonly string[] ignoreProperties;

            internal PropertySynchronizer(T source, T target, string[] ignoreProperties)
            {
                this.source = source;
                this.target = target;
                this.ignoreProperties = ignoreProperties;
                this.source.PropertyChanged += OnSourcePropertyChanged;
            }

            public void Dispose()
            {
                this.source.PropertyChanged -= OnSourcePropertyChanged;
            }

            private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.ignoreProperties?.Contains(e.PropertyName) == true)
                {
                    return;
                }
                if (string.IsNullOrEmpty(e.PropertyName))
                {
                    SynchronizeProperties(this.source, this.target, this.ignoreProperties);
                    return;
                }

                var propertyInfo = this.source.GetType().GetProperty(e.PropertyName, BindingFlags);
                if (propertyInfo == null)
                {
                    return;
                }

                var value = propertyInfo.GetValue(this.source);
                propertyInfo.SetValue(this.target, value);
            }
        }
    }
}