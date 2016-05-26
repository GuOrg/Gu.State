﻿namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>A configuration object with settings for how to copy and compare etc. instances using properties.</summary>
    public sealed class PropertiesSettings : MemberSettings<PropertyInfo>, IMemberSettings
    {
        private static readonly ConcurrentDictionary<BindingFlagsAndReferenceHandling, PropertiesSettings> Cache = new ConcurrentDictionary<BindingFlagsAndReferenceHandling, PropertiesSettings>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesSettings"/> class.
        /// </summary>
        /// <param name="ignoredProperties">The properties provided here will be ignored when the intsance of <see cref="PropertiesSettings"/> is used</param>
        /// <param name="ignoredTypes">
        /// The types to ignore
        /// </param>
        /// <param name="comparers">Custom comparers. Use this to get better performance or for custom equality for types.</param>
        /// <param name="copyers">Custom copyers.</param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        public PropertiesSettings(
            IEnumerable<PropertyInfo> ignoredProperties,
            IEnumerable<Type> ignoredTypes,
            IReadOnlyDictionary<Type, CastingComparer> comparers,
            IReadOnlyDictionary<Type, CustomCopy> copyers,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags)
            : base(ignoredProperties, ignoredTypes, comparers, copyers, referenceHandling, bindingFlags)
        {
        }

        /// <summary>Gets a collection or ignored properties.</summary>
        public IEnumerable<PropertyInfo> IgnoredProperties => this.IgnoredMembers.Keys;

        internal ConcurrentDictionary<Type, TypeErrors> TrackableErrors { get; } = new ConcurrentDictionary<Type, TypeErrors>();

        /// <summary> Create a builder for building a <see cref="PropertiesSettings"/></summary>
        /// <returns>A <see cref="PropertiesSettingsBuilder"/></returns>
        public static PropertiesSettingsBuilder Build()
        {
            return new PropertiesSettingsBuilder();
        }

        /// <summary>
        /// Creates an instance of <see cref="PropertiesSettings"/> or gets it from cache.
        /// </summary>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <returns>An instance of <see cref="PropertiesSettings"/></returns>
        public static PropertiesSettings GetOrCreate(
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags)
        {
            var key = new BindingFlagsAndReferenceHandling(bindingFlags, referenceHandling);
            return Cache.GetOrAdd(
                key,
                x => new PropertiesSettings(null, null, null, null, referenceHandling, bindingFlags));
        }

        public bool IsIgnoringProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                return true;
            }

            if (this.IsIgnoringDeclaringType(propertyInfo.DeclaringType))
            {
                return true;
            }

            return this.IgnoredMembers.GetOrAdd(propertyInfo, this.GetIsIgnoring);
        }

        public IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties(this.BindingFlags);
        }

        IEnumerable<MemberInfo> IMemberSettings.GetMembers(Type type) => this.GetProperties(type);

        bool IMemberSettings.IsIgnoringMember(MemberInfo member)
        {
            Debug.Assert(member is PropertyInfo, "member is PropertyInfo");
            return this.IsIgnoringProperty((PropertyInfo)member);
        }

        IGetterAndSetter IMemberSettings.GetOrCreateGetterAndSetter(MemberInfo member)
        {
            Debug.Assert(member is PropertyInfo, "member is PropertyInfo");
            return this.GetOrCreateGetterAndSetter((PropertyInfo)member);
        }

        internal IGetterAndSetter GetOrCreateGetterAndSetter(PropertyInfo propertyInfo)
        {
            return GetterAndSetter.GetOrCreate(propertyInfo);
        }

        private bool GetIsIgnoring(PropertyInfo propertyInfo)
        {
            foreach (var kvp in this.IgnoredMembers)
            {
                if (!kvp.Value)
                {
                    continue;
                }

                var ignoredProperty = kvp.Key;
                if (ignoredProperty.Name != propertyInfo.Name)
                {
                    continue;
                }

                if (ignoredProperty.DeclaringType?.IsAssignableFrom(propertyInfo.DeclaringType) == true)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
