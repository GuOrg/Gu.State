namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    using Gu.State.Internals;

    /// <summary>Builder for creating <see cref="FieldsSettings"/>.</summary>
    public class FieldsSettingsBuilder
    {
        private readonly HashSet<Type> ignoredTypes = new();
        private readonly HashSet<Type> immutableTypes = new();
        private readonly HashSet<FieldInfo> ignoredFields = new(MemberInfoComparer<FieldInfo>.Default);
        private readonly Dictionary<Type, IEqualityComparer> comparers = new();
        private readonly Dictionary<Type, CustomCopy> copyers = new();

        /// <summary>
        /// Create the settings object.
        /// </summary>
        /// <param name="referenceHandling">How references are handled.</param>
        /// <param name="bindingFlags">What bindingflags to use.</param>
        /// <returns>An instance of <see cref="FieldsSettings"/>.</returns>
        public FieldsSettings CreateSettings(
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags)
        {
            if (this.ignoredFields.Count == 0 &&
                this.ignoredTypes is null &&
                this.comparers.Count == 0 &&
                this.copyers.Count == 0)
            {
                return FieldsSettings.GetOrCreate(referenceHandling, bindingFlags);
            }

            return new FieldsSettings(
                this.ignoredFields,
                this.ignoredTypes,
                this.immutableTypes,
                this.comparers,
                this.copyers,
                referenceHandling,
                bindingFlags);
        }

        /// <summary>Ignore the type <typeparamref name="T"/> in the setting.</summary>
        /// <typeparam name="T">The type to ignore.</typeparam>
        /// <returns>The builder instance for chaining.</returns>
        public FieldsSettingsBuilder IgnoreType<T>()
        {
            return this.IgnoreType(typeof(T));
        }

        /// <summary>Ignore the type <paramref name="type"/> in the setting.</summary>
        /// <param name="type">The type to ignore.</param>
        /// <returns>The builder instance for chaining.</returns>
        public FieldsSettingsBuilder IgnoreType(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!this.ignoredTypes.Add(type))
            {
                var message = $"Already added type: {type.FullName}\r\n" +
                              $"Nested properties are not allowed";
                throw new ArgumentException(message);
            }

            return this;
        }

        /// <summary>Treat <typeparamref name="T"/> as immutable.</summary>
        /// <typeparam name="T">The immutable type.</typeparam>
        /// <returns>The builder instance for chaining.</returns>
        public FieldsSettingsBuilder AddImmutableType<T>()
        {
            return this.AddImmutableType(typeof(T));
        }

        /// <summary>Treat <paramref name="type"/> as immutable.</summary>
        /// <param name="type">The immutable type.</param>
        /// <returns>The builder instance for chaining.</returns>
        public FieldsSettingsBuilder AddImmutableType(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!this.immutableTypes.Add(type))
            {
                var message = $"Already added type: {type.FullName}\r\n" +
                              $"Nested Fields are not allowed";
                throw new ArgumentException(message);
            }

            return this;
        }

        /// <summary>Ignore the field <paramref name="fieldInfo"/> in the setting.</summary>
        /// <param name="fieldInfo">The field to ignore.</param>
        /// <returns>The builder instance for chaining.</returns>
        public FieldsSettingsBuilder AddIgnoredField(FieldInfo fieldInfo)
        {
            if (fieldInfo is null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            if (!this.ignoredFields.Add(fieldInfo))
            {
                var message = $"Already added property: {fieldInfo.DeclaringType?.FullName}.{fieldInfo.Name}\r\n" +
                              $"Nested Fields are not allowed";
                throw new ArgumentException(message);
            }

            return this;
        }

        /// <summary>Ignore the field named <paramref name="name"/> for type <typeparamref name="TSource"/> in the setting.</summary>
        /// <typeparam name="TSource">The type with the field to ignore.</typeparam>
        /// <param name="name">The name of field to ignore.</param>
        /// <returns>The builder instance for chaining.</returns>
        public FieldsSettingsBuilder AddIgnoredField<TSource>(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var fieldInfo = typeof(TSource).GetField(name, Constants.DefaultFieldBindingFlags);
            if (fieldInfo is null)
            {
                var message = $"{name} must be a field on {typeof(TSource).Name}\r\n" +
                              $"Nested Fields are not allowed";
                throw new ArgumentException(message);
            }

            return this.AddIgnoredField(fieldInfo);
        }

        /// <summary>Add a custom comparer for type <typeparamref name="T"/> in the setting.</summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/>.</param>
        /// <returns>The builder instance for chaining.</returns>
        public FieldsSettingsBuilder AddComparer<T>(IEqualityComparer<T> comparer)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            this.comparers[typeof(T)] = comparer as IEqualityComparer ?? CastingComparer.Create(comparer);
            return this;
        }

        /// <summary>Provide a custom copy implementation for <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The type to copy.</typeparam>
        /// <param name="copyMethod">
        /// This method gets passed the source value, target value and returns the updated target value or a new target value.
        /// </param>
        /// <returns>Self.</returns>
        public FieldsSettingsBuilder AddCustomCopy<T>(Func<T, T, T> copyMethod)
        {
            if (copyMethod is null)
            {
                throw new ArgumentNullException(nameof(copyMethod));
            }

            this.copyers[typeof(T)] = CustomCopy.Create(copyMethod);
            return this;
        }
    }
}
