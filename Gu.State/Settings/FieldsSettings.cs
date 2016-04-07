namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    public class FieldsSettings : MemberSettings<FieldInfo>, IIgnoringFields
    {
        private static readonly ConcurrentDictionary<BindingFlagsAndReferenceHandling, FieldsSettings> Cache = new ConcurrentDictionary<BindingFlagsAndReferenceHandling, FieldsSettings>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsSettings"/> class.
        /// </summary>
        /// <param name="ignoredFields">The fields provided here will be ignored when the intsance of <see cref="FieldsSettings"/> is used</param>
        /// <param name="ignoredTypes">The types to ignore</param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private.
        /// </param>
        public FieldsSettings(
            IEnumerable<FieldInfo> ignoredFields,
            IEnumerable<Type> ignoredTypes,
            BindingFlags bindingFlags,
            ReferenceHandling referenceHandling)
            : base(ignoredFields, ignoredTypes, bindingFlags, referenceHandling)
        {
        }

        public IEnumerable<FieldInfo> IgnoredFields => this.IgnoredMembers.Keys;

        public static FieldsSettingsBuilder Build()
        {
            return new FieldsSettingsBuilder();
        }

        /// <summary>
        /// Creates an instance of <see cref="FieldsSettings"/>. or gets from cache.
        /// </summary>
        /// <param name="bindingFlags">
        /// The binding flags to use when getting fields
        /// Default is BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        /// </param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub fields are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        /// <returns>An instance of <see cref="FieldsSettings"/></returns>
        public static FieldsSettings GetOrCreate(BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags, ReferenceHandling referenceHandling = ReferenceHandling.Throw)
        {
            var key = new BindingFlagsAndReferenceHandling(bindingFlags, referenceHandling);
            return Cache.GetOrAdd(key, x => new FieldsSettings(null, null, bindingFlags, referenceHandling));
        }

        public bool IsIgnoringField(FieldInfo fieldInfo)
        {
            if (fieldInfo == null || fieldInfo.IsEventField() || typeof(IEnumerator).IsAssignableFrom(fieldInfo.DeclaringType))
            {
                return true;
            }

            if (this.IsIgnoringDeclaringType(fieldInfo.DeclaringType))
            {
                return true;
            }

            return this.IgnoredMembers.ContainsKey(fieldInfo);
        }

        internal override IGetterAndSetter GetOrCreateGetterAndSetter(FieldInfo propertyInfo)
        {
            return GetterAndSetter.GetOrCreate(propertyInfo);
        }
    }
}