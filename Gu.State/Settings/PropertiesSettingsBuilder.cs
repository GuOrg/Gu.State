namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Gu.State.Internals;

    /// <summary>Builder for creating <see cref="PropertiesSettings"/>.</summary>
    public class PropertiesSettingsBuilder
    {
        private readonly HashSet<Type> ignoredTypes = new HashSet<Type>();
        private readonly HashSet<Type> immutableTypes = new HashSet<Type>();
        private readonly HashSet<PropertyInfo> ignoredProperties = new HashSet<PropertyInfo>(MemberInfoComparer<PropertyInfo>.Default);
        private readonly Dictionary<Type, IEqualityComparer> comparers = new Dictionary<Type, IEqualityComparer>();
        private readonly Dictionary<Type, CustomCopy> copyers = new Dictionary<Type, CustomCopy>();

        /// <summary>
        /// Create the settings object.
        /// </summary>
        /// <param name="referenceHandling">How references are handled.</param>
        /// <param name="bindingFlags">What bindingflags to use.</param>
        /// <returns>An instance of <see cref="PropertiesSettings"/>.</returns>
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
                this.immutableTypes,
                this.comparers,
                this.copyers,
                referenceHandling,
                bindingFlags);
        }

        /// <summary>Treat <typeparamref name="T"/> as immutable.</summary>
        /// <typeparam name="T">The immutable type.</typeparam>
        /// <returns>The builder instance for chaining.</returns>
        public PropertiesSettingsBuilder AddImmutableType<T>()
        {
            return this.AddImmutableType(typeof(T));
        }

        /// <summary>Treat <paramref name="type"/> as immutable.</summary>
        /// <param name="type">The immutable type.</param>
        /// <returns>The builder instance for chaining.</returns>
        public PropertiesSettingsBuilder AddImmutableType(Type type)
        {
            if (!this.immutableTypes.Add(type))
            {
                var message = $"Already added type: {type.FullName}\r\n" +
                              $"Nested Fields are not allowed";
                throw new ArgumentException(message);
            }

            return this;
        }

        /// <summary>Ignore the type <typeparamref name="T"/> in the setting.</summary>
        /// <typeparam name="T">The type to ignore.</typeparam>
        /// <returns>The builder instance for chaining.</returns>
        public PropertiesSettingsBuilder IgnoreType<T>()
        {
            return this.IgnoreType(typeof(T));
        }

        /// <summary>Ignore the type <paramref name="type"/> in the setting.</summary>
        /// <param name="type">The type to ignore.</param>
        /// <returns>The builder instance for chaining.</returns>
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

        /// <summary>Ignore the property <paramref name="property"/> in the setting.</summary>
        /// <param name="property">The property to ignore.</param>
        /// <returns>The builder instance for chaining.</returns>
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

        /// <summary>Ignore the property named <paramref name="name"/> for type <typeparamref name="TSource"/> in the setting.</summary>
        /// <typeparam name="TSource">The type with the property to ignore.</typeparam>
        /// <param name="name">The name of property to ignore.</param>
        /// <returns>The builder instance for chaining.</returns>
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
        /// Sample: AddExplicitProperty{<typeparamref name="TSource"/>}(x => x.Bar).
        /// </summary>
        /// <typeparam name="TSource">The type of the parameter in the lambda.</typeparam>
        /// <param name="property">Sample x => x.SomeProperty.</param>
        /// <returns>Returns self for chaining.</returns>
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

        /// <summary>Ignore indexers for type <typeparamref name="T"/> in the setting.</summary>
        /// <typeparam name="T">The type with for which indexers are ignored.</typeparam>
        /// <returns>The builder instance for chaining.</returns>
        public PropertiesSettingsBuilder IgnoreIndexersFor<T>()
        {
            foreach (var indexer in typeof(T).GetProperties(Constants.DefaultFieldBindingFlags).Where(x => x.GetIndexParameters().Length > 0))
            {
                this.IgnoreProperty(indexer);
            }

            return this;
        }

        /// <summary>Add a custom comparer for type <typeparamref name="T"/> in the setting.</summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The builder instance for chaining.</returns>
        public PropertiesSettingsBuilder AddComparer<T>(IEqualityComparer<T> comparer)
        {
            this.comparers[typeof(T)] = comparer as IEqualityComparer ?? CastingComparer.Create(comparer);
            return this;
        }

        /// <summary>Provide a custom copy implementation for <typeparamref name="T"/>.</summary>
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
