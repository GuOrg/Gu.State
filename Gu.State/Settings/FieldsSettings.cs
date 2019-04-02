namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// A configuration object with settings for how to copy and compare etc. instances using fields.
    /// The setting should be cached between calls for performance.
    /// </summary>
    public sealed class FieldsSettings : MemberSettings
    {
        private static readonly ConcurrentDictionary<BindingFlagsAndReferenceHandling, FieldsSettings> Cache = new ConcurrentDictionary<BindingFlagsAndReferenceHandling, FieldsSettings>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsSettings"/> class.
        /// </summary>
        /// <param name="ignoredFields">The fields provided here will be ignored when the intsance of <see cref="FieldsSettings"/> is used. Can be null.</param>
        /// <param name="ignoredTypes">The types to ignore.</param>
        /// <param name="immutableTypes">A collection of types to treat as immutable. Can be null.</param>
        /// <param name="comparers">Custom comparers. Use this to get better performance or for custom equality for types.</param>
        /// <param name="copyers">Custom copyers.</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private.
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting properties.</param>
        public FieldsSettings(
            IEnumerable<FieldInfo> ignoredFields,
            IEnumerable<Type> ignoredTypes,
            IEnumerable<Type> immutableTypes,
            IReadOnlyDictionary<Type, CastingComparer> comparers,
            IReadOnlyDictionary<Type, CustomCopy> copyers,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags)
            : base(ignoredFields, ignoredTypes, immutableTypes, comparers, copyers, referenceHandling, bindingFlags)
        {
        }

        /// <summary>
        /// Gets the default settings
        /// - ReferenceHandling.Structural
        /// - Bindingflags.Instance | BindingFlags.NonPublic | BindingFlags.Public.
        /// </summary>
        public static FieldsSettings Default => GetOrCreate();

        /// <summary>Gets a collection of ignored fields.</summary>
        public IEnumerable<FieldInfo> IgnoredFields => this.IgnoredMembers.Keys.Cast<FieldInfo>();

        /// <summary> Create a builder for building a <see cref="FieldsSettings"/>.</summary>
        /// <returns>A <see cref="FieldsSettingsBuilder"/>.</returns>
        public static FieldsSettingsBuilder Build()
        {
            return new FieldsSettingsBuilder();
        }

        /// <summary>
        /// Creates an instance of <see cref="FieldsSettings"/>. or gets from cache.
        /// </summary>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub fields are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private.
        /// </param>
        /// <param name="bindingFlags">
        /// The binding flags to use when getting fields
        /// Default is BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic.
        /// </param>
        /// <returns>An instance of <see cref="FieldsSettings"/>.</returns>
        public static FieldsSettings GetOrCreate(
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags)
        {
            var key = new BindingFlagsAndReferenceHandling(bindingFlags, referenceHandling);
            return Cache.GetOrAdd(key, x => new FieldsSettings(null, null, null, null, null, referenceHandling, bindingFlags));
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

        public override IEnumerable<MemberInfo> GetMembers(Type type)
        {
            while (type != null &&
                   type != typeof(object))
            {
                foreach (var field in type.GetFields(this.BindingFlags))
                {
                    yield return field;
                }

                type = type.BaseType;
            }
        }

        public override bool IsIgnoringMember(MemberInfo member)
        {
            Debug.Assert(member is FieldInfo, "member is FieldInfo");
            return this.IsIgnoringField((FieldInfo)member);
        }

        internal override IGetterAndSetter GetOrCreateGetterAndSetter(MemberInfo member)
        {
            Debug.Assert(member is FieldInfo, "member is FieldInfo");
            return this.GetOrCreateGetterAndSetter((FieldInfo)member);
        }

        private IGetterAndSetter GetOrCreateGetterAndSetter(FieldInfo propertyInfo)
        {
            return GetterAndSetter.GetOrCreate(propertyInfo);
        }
    }
}