namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    public class FieldsSettings : MemberSettings<FieldInfo>, IIgnoringFields
    {
        private static readonly ConcurrentDictionary<BindingFlagsAndReferenceHandling, FieldsSettings> Cache =
            new ConcurrentDictionary<BindingFlagsAndReferenceHandling, FieldsSettings>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsSettings"/> class.
        /// </summary>
        /// <param name="ignoredFields">The fields provided here will be ignored when the intsance of <see cref="FieldsSettings"/> is used</param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private.
        /// </param>
        public FieldsSettings(
            IEnumerable<FieldInfo> ignoredFields,
            BindingFlags bindingFlags,
            ReferenceHandling referenceHandling)
            : base(ignoredFields, bindingFlags, referenceHandling)
        {
        }

        public IEnumerable<FieldInfo> IgnoredFields => this.IgnoredMembers;

        /// <summary>
        /// Creates an instance of <see cref="FieldsSettings"/>.
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for</typeparam>
        /// <param name="x">The instance to copy property values from</param>
        /// <param name="y">The instance to copy property values to</param>
        /// <param name="bindingFlags">The binding flags to use when getting fields</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub fields are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        /// <param name="excludedFields">Names of fields on <typeparamref name="T"/> to exclude from copying</param>
        /// <returns>An instance of <see cref="FieldsSettings"/></returns>
        public static FieldsSettings Create<T>(
            T x,
            T y,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw,
            params string[] excludedFields)
        {
            var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
            return Create(type, bindingFlags, referenceHandling, excludedFields);
        }

        /// <summary>
        /// Creates an instance of <see cref="FieldsSettings"/>.
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for</typeparam>
        /// <param name="bindingFlags">The binding flags to use when getting fields</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub fields are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        /// <param name="excludedFields">Names of fields on <typeparamref name="T"/> to exclude from copying</param>
        /// <returns>An instance of <see cref="FieldsSettings"/></returns>
        public static FieldsSettings Create<T>(
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw,
            params string[] excludedFields)
        {
            return Create(typeof(T), bindingFlags, referenceHandling, excludedFields);
        }

        /// <summary>
        /// Creates an instance of <see cref="FieldsSettings"/>.
        /// </summary>
        /// <param name="type">The type to get ignore fields for settings for</param>
        /// <param name="bindingFlags">The binding flags to use when getting fields</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub fields are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        /// <param name="excludedFields">Names of fields on <paramref name="type"/> to exclude from copying</param>
        /// <returns>An instance of <see cref="PropertiesSettings"/></returns>
        public static FieldsSettings Create(
            Type type,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw,
            params string[] excludedFields)
        {
            var ignoreFields = type.GetIgnoreFields(bindingFlags, excludedFields);
            if (ignoreFields == null || ignoreFields.Count == 0)
            {
                return GetOrCreate(bindingFlags, referenceHandling);
            }

            return new FieldsSettings(ignoreFields, bindingFlags, referenceHandling);
        }

        public static FieldsSettings GetOrCreate(BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags, ReferenceHandling referenceHandling = ReferenceHandling.Throw)
        {
            var key = new BindingFlagsAndReferenceHandling(bindingFlags, referenceHandling);
            return Cache.GetOrAdd(key, x => new FieldsSettings(null, bindingFlags, referenceHandling));
        }

        public bool IsIgnoringField(FieldInfo fieldInfo)
        {
            if (fieldInfo == null || fieldInfo.IsEventField())
            {
                return true;
            }

            if (this.IsIgnoringDeclaringType(fieldInfo.DeclaringType))
            {
                return true;
            }

            return this.IsIgnoringMember(fieldInfo);
        }
    }
}