namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Gu.State.Internals;

    public class FieldsSettingsBuilder
    {
        private readonly HashSet<Type> ignoredTypes = new HashSet<Type>();
        private readonly HashSet<FieldInfo> ignoredFields = new HashSet<FieldInfo>(MemberInfoComparer<FieldInfo>.Default);
        private readonly Dictionary<Type, CastingComparer> comparers = new Dictionary<Type, CastingComparer>();
        private readonly Dictionary<Type, CustomCopy> copyers = new Dictionary<Type, CustomCopy>();

        public FieldsSettings CreateSettings(ReferenceHandling referenceHandling = ReferenceHandling.Throw, BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags)
        {
            if (this.ignoredFields.Count == 0 && this.ignoredTypes == null)
            {
                return FieldsSettings.GetOrCreate(bindingFlags, referenceHandling);
            }

            return new FieldsSettings(this.ignoredFields, this.ignoredTypes, this.comparers, this.copyers, bindingFlags, referenceHandling);
        }

        public FieldsSettingsBuilder AddImmutableType<T>()
        {
            return this.AddImmutableType(typeof(T));
        }

        public FieldsSettingsBuilder AddImmutableType(Type type)
        {
            if (!this.ignoredTypes.Add(type))
            {
                var message = $"Already added type: {type.FullName}\r\n" +
                              $"Nested Fields are not allowed";
                throw new ArgumentException(message);
            }

            return this;
        }

        public FieldsSettingsBuilder AddIgnoredField(FieldInfo fieldInfo)
        {
            if (!this.ignoredFields.Add(fieldInfo))
            {
                var message = $"Already added property: {fieldInfo.DeclaringType?.FullName}.{fieldInfo.Name}\r\n" +
                              $"Nested Fields are not allowed";
                throw new ArgumentException(message);
            }

            return this;
        }

        public FieldsSettingsBuilder AddIgnoredField<TSource>(string name)
        {
            var fieldInfo = typeof(TSource).GetField(name, Constants.DefaultFieldBindingFlags);
            if (fieldInfo == null)
            {
                var message = $"{name} must be a field on {typeof(TSource).Name}\r\n" +
                              $"Nested Fields are not allowed";
                throw new ArgumentException(message);
            }

            return this.AddIgnoredField(fieldInfo);
        }

        public FieldsSettingsBuilder AddComparer<T>(IEqualityComparer<T> comparer)
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
        public FieldsSettingsBuilder AddCustomCopy<T>(Func<T, T, T> copyMethod)
        {
            this.copyers[typeof(T)] = CustomCopy.Create(copyMethod);
            return this;
        }
    }
}
