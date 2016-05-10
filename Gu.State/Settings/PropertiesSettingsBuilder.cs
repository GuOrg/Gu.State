namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Gu.State.Internals;

    public class PropertiesSettingsBuilder
    {
        private readonly HashSet<Type> ignoredTypes = new HashSet<Type>();
        private readonly HashSet<PropertyInfo> ignoredProperties = new HashSet<PropertyInfo>(MemberInfoComparer<PropertyInfo>.Default);
        private readonly Dictionary<Type, CastingComparer> comparers = new Dictionary<Type, CastingComparer>();
        private readonly Dictionary<Type, CustomCopy> copyers = new Dictionary<Type, CustomCopy>();

        public PropertiesSettings CreateSettings(
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags)
        {
            if (this.ignoredProperties.Count == 0 &&
                this.ignoredTypes == null &&
                this.comparers.Count == 0 &&
                this.copyers.Count == 0)
            {
                return PropertiesSettings.GetOrCreate(referenceHandling, bindingFlags);
            }

            return new PropertiesSettings(
                this.ignoredProperties,
                this.ignoredTypes,
                this.comparers,
                this.copyers,
                referenceHandling,
                bindingFlags);
        }

        public PropertiesSettingsBuilder IgnoreType<T>()
        {
            return this.IgnoreType(typeof(T));
        }

        public PropertiesSettingsBuilder IgnoreType(Type type)
        {
            if (!this.ignoredTypes.Add(type))
            {
                var message = $"Already added type: {type.FullName}\r\n" +
                              $"Nested properties are not allowed";
                throw new ArgumentException(message);
            }

            return this;
        }

        public PropertiesSettingsBuilder IgnoreProperty(PropertyInfo property)
        {
            if (!this.ignoredProperties.Add(property))
            {
                var message = $"Already added property: {property.DeclaringType?.FullName}.{property.Name}\r\n" +
                              $"Nested properties are not allowed";
                throw new ArgumentException(message);
            }

            return this;
        }

        public PropertiesSettingsBuilder IgnoreProperty<TSource>(string name)
        {
            var propertyInfo = typeof(TSource).GetProperty(name, Constants.DefaultFieldBindingFlags);
            if (propertyInfo == null)
            {
                var message = $"{name} must be a property on {typeof(TSource).Name}\r\n" +
                              $"Nested properties are not allowed";
                throw new ArgumentException(message);
            }

            return this.IgnoreProperty(propertyInfo);
        }

        /// <summary>
        /// Sample: AddExplicitProperty{<typeparamref name="TSource"/>}(x => x.Bar)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="property"></param>
        public PropertiesSettingsBuilder IgnoreProperty<TSource>(Expression<Func<TSource, object>> property)
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
            {
                if (property.Body.NodeType == ExpressionType.Convert)
                {
                    memberExpression = (property.Body as UnaryExpression)?.Operand as MemberExpression;
                }
            }

            if (memberExpression == null)
            {
                var message = $"{nameof(property)} must be a property expression like foo => foo.Bar\r\n" +
                              $"Nested properties are not allowed";
                throw new ArgumentException(message);
            }

            if (memberExpression.Expression.NodeType != ExpressionType.Parameter)
            {
                var message = $"{nameof(property)} must be a property expression like foo => foo.Bar\r\n" +
                              $"Nested properties are not allowed";
                throw new ArgumentException(message);
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                var message = $"{nameof(property)} must be a property expression like foo => foo.Bar";
                throw new ArgumentException(message);
            }

            this.IgnoreProperty(propertyInfo);
            return this;
        }

        public PropertiesSettingsBuilder IgnoreIndexersFor<T>()
        {
            foreach (var indexer in typeof(T).GetProperties(Constants.DefaultFieldBindingFlags).Where(x => x.GetIndexParameters().Length > 0))
            {
                this.IgnoreProperty(indexer);
            }

            return this;
        }

        public PropertiesSettingsBuilder AddComparer<T>(IEqualityComparer<T> comparer)
        {
            this.comparers[typeof(T)] = CastingComparer.Create(comparer);
            return this;
        }

        /// <summary>
        /// Custom copy implementation.
        /// </summary>
        /// <typeparam name="T">The type to copy.</typeparam>
        /// <param name="copyMethod">
        /// This method gets passed the source value, target value and returns the updated target value or a new target value.
        /// </param>
        /// <returns>Self.</returns>
        public PropertiesSettingsBuilder AddCustomCopy<T>(Func<T, T, T> copyMethod)
        {
            this.copyers[typeof(T)] = CustomCopy.Create(copyMethod);
            return this;
        }
    }
}
