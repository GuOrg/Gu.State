namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>A configuration object with settings for how to copy and compare etc. instances using fields.</summary>
    public sealed class FieldsSettings : MemberSettings<FieldInfo>, IMemberSettings
    {
        private static readonly ConcurrentDictionary<BindingFlagsAndReferenceHandling, FieldsSettings> Cache = new ConcurrentDictionary<BindingFlagsAndReferenceHandling, FieldsSettings>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsSettings"/> class.
        /// </summary>
        /// <param name="ignoredFields">The fields provided here will be ignored when the intsance of <see cref="FieldsSettings"/> is used</param>
        /// <param name="ignoredTypes">The types to ignore</param>
        /// <param name="comparers">Custom comparers. Use this to get better performance or for custom equality for types.</param>
        /// <param name="copyers">Custom copyers.</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private.
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        public FieldsSettings(
            IEnumerable<FieldInfo> ignoredFields,
            IEnumerable<Type> ignoredTypes,
            IReadOnlyDictionary<Type, CastingComparer> comparers,
            IReadOnlyDictionary<Type, CustomCopy> copyers,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags)
            : base(ignoredFields, ignoredTypes, comparers, copyers, referenceHandling, bindingFlags)
        {
        }

        /// <summary>Gets a collection or ignored fields.</summary>
        public IEnumerable<FieldInfo> IgnoredFields => this.IgnoredMembers.Keys;

        /// <summary> Create a builder for building a <see cref="FieldsSettings"/></summary>
        /// <returns>A <see cref="FieldsSettingsBuilder"/></returns>
        public static FieldsSettingsBuilder Build()
        {
            return new FieldsSettingsBuilder();
        }

        /// <summary>
        /// Creates an instance of <see cref="FieldsSettings"/>. or gets from cache.
        /// </summary>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub fields are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        /// <param name="bindingFlags">
        /// The binding flags to use when getting fields
        /// Default is BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        /// </param>
        /// <returns>An instance of <see cref="FieldsSettings"/></returns>
        public static FieldsSettings GetOrCreate(
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags)
        {
            var key = new BindingFlagsAndReferenceHandling(bindingFlags, referenceHandling);
            return Cache.GetOrAdd(key, x => new FieldsSettings(null, null, null, null, referenceHandling, bindingFlags));
        }

        /// <summary>Gets if the <paramref name="fieldInfo"/> is ignored.</summary>
        /// <param name="fieldInfo">The property to check.</param>
        /// <returns>A value indicating if <paramref name="fieldInfo"/> is ignored.</returns>
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

        IEnumerable<MemberInfo> IMemberSettings.GetMembers(Type type)
        {
            return type.GetFields(this.BindingFlags);
        }

        bool IMemberSettings.IsIgnoringMember(MemberInfo member)
        {
            Debug.Assert(member is FieldInfo, "member is FieldInfo");
            return this.IsIgnoringField(member as FieldInfo);
        }

        IGetterAndSetter IMemberSettings.GetOrCreateGetterAndSetter(MemberInfo member)
        {
            Debug.Assert(member is FieldInfo, "member is FieldInfo");
            return this.GetOrCreateGetterAndSetter(member as FieldInfo);
        }

        private IGetterAndSetter GetOrCreateGetterAndSetter(FieldInfo propertyInfo)
        {
            return GetterAndSetter.GetOrCreate(propertyInfo);
        }
    }
}